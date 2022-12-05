using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Grabbable : MonoBehaviourPun {
    private XRGrabInteractable Interactable;

    private void Start() {
        Reset();

        Interactable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy() {
        Interactable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args) {
        if (args.interactableObject.interactorsSelecting.Count == 1) {
            photonView.RequestOwnership();
        }
    }

    private void Reset() {
        Interactable = Interactable == null ? GetComponent<XRGrabInteractable>() : Interactable;
        photonView.OwnershipTransfer = OwnershipOption.Takeover;
    }
}
