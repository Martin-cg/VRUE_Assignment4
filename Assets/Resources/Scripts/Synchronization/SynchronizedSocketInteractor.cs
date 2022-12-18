using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRSocketInteractor))]
public class SynchronizedSocketInteractor : SynchronizedRoomObject, IXRSelectFilter {
    private XRSocketInteractor Socket;
    private SynchronizedRoomProperty<int[]> CurrentInteractables;
    private int[] OldInteractables = new int[0];

    public bool canProcess => true;

    protected virtual void Reset() {
        FindSocket();
    }

    public virtual void OnDestroy() {
        if (Socket) {
            Socket.selectEntered.RemoveListener(OnSelectEnter);
            Socket.selectExited.RemoveListener(OnSelectExit);

            Socket.selectFilters.Remove(this);
        }
    }

    protected override void Start() {
        base.Start();

        FindSocket();

        CurrentInteractables = RegisterProperty<int[]>(nameof(CurrentInteractables), null);
        CurrentInteractables.ValueChanged += OnCurrentInteractablesRoomPropertyChanged;

        Socket.selectEntered.AddListener(OnSelectEnter);
        Socket.selectExited.AddListener(OnSelectExit);

        Socket.selectFilters.Add(this);

        foreach (var interactable in Socket.interactablesSelected) {
            OnLocalInteractableEntered(interactable);
        }
    }

    private void OnCurrentInteractablesRoomPropertyChanged(object sender, int[] sceneViewIds) {
        var toRemove = OldInteractables.Except(sceneViewIds);
        var toAdd = sceneViewIds.Except(OldInteractables);

        foreach (var viewId in toRemove) {
            var photonView = PhotonView.Find(viewId);
            var interactable = photonView.GetComponent<XRGrabInteractable>();
            OnRemoteInteractableExited(interactable);
        }

        foreach (var viewId in toAdd) {
            var photonView = PhotonView.Find(viewId);
            var interactable = photonView.GetComponent<XRGrabInteractable>();
            OnRemoteInteractableEntered(interactable);
        }

        OldInteractables = sceneViewIds;
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

    private void OnRemoteInteractableExited(IXRSelectInteractable interactable) {
        if (!Socket.hasSelection) {
            return;
        }

        Socket.interactionManager.SelectExit(Socket, interactable);
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

        var viewIds = Socket.interactablesSelected.Select(interactable => PhotonView.Get(interactable.transform).sceneViewId).ToArray();
        CurrentInteractables.SetValue(viewIds);
    }

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable) {
        var photonView = PhotonView.Get(interactable.transform);
        return photonView.IsMine || CurrentInteractables.Value.Contains(photonView.ViewID);
    }
}
