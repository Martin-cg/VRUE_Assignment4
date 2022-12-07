using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Grabbable : MonoBehaviourPun {
    protected XRGrabInteractable Interactable;
    public UnityEvent Grabbed = new();
    public UnityEvent Released = new();

    protected virtual void Reset() {
        Init();
    }

    protected virtual void Awake() {
        Init();
    }

    private void Init() {
        Interactable = Interactable == null ? GetComponent<XRGrabInteractable>() : Interactable;
        photonView.OwnershipTransfer = OwnershipOption.Takeover;
    }

    protected virtual void OnEnable() {
        Interactable.selectEntered.AddListener(OnSelectEntered);
        Interactable.selectExited.AddListener(OnSelectExited);
    }
    private void OnDestroy() {
        Interactable.selectEntered.RemoveListener(OnSelectEntered);
        Interactable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args) {
        if (!CanGrab(Character.Local, args.interactorObject)) {
            return;
        }

        photonView.RequestOwnership();
    }

    private void OnSelectExited(SelectExitEventArgs args) {
    }

    public virtual bool CanGrab(Character character, IXRSelectInteractor interactor) {
        return true;
    }

    protected virtual void OnGrabbed() {
        Grabbed.Invoke();
    }

    protected virtual void OnReleased() {
        Released.Invoke();
    }

    private class GrabbableSelectFilter : IXRSelectFilter {
        public Grabbable Grabbable;

        public GrabbableSelectFilter(Grabbable grabbable) {
            Grabbable = grabbable;
        }

        public bool canProcess => true;

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable) {
            return Grabbable.CanGrab(Character.Local, interactor);
        }
    }
}
