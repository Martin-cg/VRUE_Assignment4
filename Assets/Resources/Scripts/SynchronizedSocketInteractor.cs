using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class SynchronizedSocketInteractor : SynchronizedRoomObject {
    private XRSocketInteractor Socket;
    private const string PropertyKey = "CurrentInteractableSceneViewId";

    protected virtual void Reset() {
        FindSocket();
    }

    public virtual void OnDestroy() {
        if (Socket) {
            Socket.selectEntered.RemoveListener(OnSelectEnter);
            Socket.selectExited.RemoveListener(OnSelectExit);
        }
    }

    protected override void Start() {
        base.Start();

        FindSocket();

        RegisterProperty<int?>(PropertyKey, null, sceneViewId => {
            if (sceneViewId.HasValue) {
                var photonView = PhotonView.Find(sceneViewId.Value);
                var interactable = photonView.GetComponent<XRGrabInteractable>();
                OnRemoteInteractableEntered(interactable);
            } else {
                OnRemoteInteractableExited();
            }
        });

        Socket.selectEntered.AddListener(OnSelectEnter);
        Socket.selectExited.AddListener(OnSelectExit); 

        if (Socket.firstInteractableSelected != null) {
            OnLocalInteractableEntered(Socket.firstInteractableSelected);
        }
    }

    private void FindSocket() {
        Socket = Socket == null ? GetComponent<XRSocketInteractor>() : Socket;
        Socket = Socket == null ? GetComponentInChildren<XRSocketInteractor>() : Socket;
    }

    private void OnSelectEnter(SelectEnterEventArgs args) {
        OnLocalInteractableEntered(args.interactableObject);
    }

    private void OnSelectExit(SelectExitEventArgs args) {
        OnLocalInteractableExited();
    }

    private void OnRemoteInteractableEntered(IXRSelectInteractable interactable) {
        if (interactable == Socket.firstInteractableSelected) {
            return;
        }

        Socket.interactionManager.SelectEnter(Socket, interactable);
        interactable.transform.parent = Socket.attachTransform;
    }

    private void OnRemoteInteractableExited() {
        if (!Socket.hasSelection) {
            return;
        }

        var interactable = Socket.firstInteractableSelected;
        // TODO: what of this madness is actually required
        Socket.socketActive = false;
        Socket.allowHover = false;
        Socket.allowSelect = false;
        var layers = Socket.interactionLayers;
        Socket.interactionLayers = 0;
        Socket.interactionManager.CancelInteractorSelection((IXRSelectInteractor) Socket);
        Socket.interactionManager.CancelInteractableSelection(interactable);
        Socket.interactionManager.CancelInteractorHover((IXRHoverInteractor) Socket);
        Socket.interactionManager.CancelInteractableHover((IXRHoverInteractable) interactable);
        Socket.socketActive = true;
        Socket.allowHover = true;
        Socket.allowSelect = true;
        Socket.interactionLayers = layers;
        interactable.transform.parent = null;
    }

    private void OnLocalInteractableEntered(IXRSelectInteractable interactable) {
        var photonView = PhotonView.Get(interactable.transform);
        SetProperty<int?>(PropertyKey, photonView.ViewID);
    }

    private void OnLocalInteractableExited() {
        SetProperty<int?>(PropertyKey, null);
    }
}
