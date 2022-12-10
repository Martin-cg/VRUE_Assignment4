using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonRigidbodyView))]
public class Grabbable : MonoBehaviourPun {
    protected XRGrabInteractable Interactable;

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
    protected virtual void OnDisable() {
        Interactable.selectEntered.RemoveListener(OnSelectEntered);
        Interactable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args) {
        if (args.interactorObject.transform.GetComponentInParent<XRRig>() == null) {
            return;
        }

        photonView.RequestOwnership();
        // RequestOwnership does not update IsMine immediatly, which confuses our other scripts for a few frames.
        typeof(PhotonView).GetProperty(nameof(photonView.IsMine)).SetValue(photonView, true);
        Debug.Assert(photonView.IsMine);
    }

    private void OnSelectExited(SelectExitEventArgs args) {
    }
}
