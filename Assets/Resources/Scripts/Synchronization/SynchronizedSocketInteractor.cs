using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRSocketInteractor))]
public class SynchronizedSocketInteractor : SynchronizedRoomObject, IXRSelectFilter {
    private XRSocketInteractor Socket;
    private SynchronizedRoomProperty<int?> CurrentInteractable;

    public bool canProcess => true;

    protected virtual void Reset() {
        FindSocket();
    }

    public virtual void OnDestroy() {
        if (Socket) {
            Socket.selectEntered.RemoveListener(OnSelectEnter);
            Socket.selectExited.RemoveListener(OnSelectExit);

            // Socket.hoverFilters.Remove(this);
            Socket.selectFilters.Remove(this);
        }
    }

    protected override void Start() {
        base.Start();

        FindSocket();

        CurrentInteractable = RegisterProperty<int?>(nameof(CurrentInteractable), null);
        CurrentInteractable.ValueChanged += OnCurrentInteractableRoomPropertyChanged;

        Socket.selectEntered.AddListener(OnSelectEnter);
        Socket.selectExited.AddListener(OnSelectExit);

        // Socket.hoverFilters.Add(this);
        Socket.selectFilters.Add(this);

        if (Socket.firstInteractableSelected != null) {
            OnLocalInteractableEntered(Socket.firstInteractableSelected);
        }
    }

    private void OnCurrentInteractableRoomPropertyChanged(object sender, int? sceneViewId) {
        if (sceneViewId.HasValue) {
            var photonView = PhotonView.Find(sceneViewId.Value);
            var interactable = photonView.GetComponent<XRGrabInteractable>();
            OnRemoteInteractableEntered(interactable);
        } else {
            OnRemoteInteractableExited();
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
        // interactable.transform.parent = Socket.attachTransform;
    }

    private void OnRemoteInteractableExited() {
        if (!Socket.hasSelection) {
            return;
        }

        var interactable = Socket.firstInteractableSelected;
        // TODO: what of this madness is actually required
        Socket.interactionManager.SelectExit(Socket, interactable);
        /*Socket.socketActive = false;
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
        Socket.interactionLayers = layers;*/
        // interactable.transform.parent = null;
    }

    private void OnLocalInteractableEntered(IXRSelectInteractable interactable) {
        if (Socket && Socket.gameObject.activeInHierarchy) {
            StartCoroutine(UpdateSynchronizedProperty());
        }
    }

    private void OnLocalInteractableExited() {
        if (Socket && Socket.gameObject.activeInHierarchy) {
            StartCoroutine(UpdateSynchronizedProperty());
        }
    }

    private IEnumerator UpdateSynchronizedProperty() {
        yield return null;

        switch (Socket.firstInteractableSelected) {
            case IXRSelectInteractable interactable:
                var photonView = PhotonView.Get(interactable.transform);
                CurrentInteractable.SetValue(photonView.ViewID);
                break;
            case null:
                CurrentInteractable.SetValue(null);
                break;
        }
    }

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable) {
        var photonView = PhotonView.Get(interactable.transform);
        return photonView.IsMine || photonView.ViewID == CurrentInteractable.Value;
    }
}
