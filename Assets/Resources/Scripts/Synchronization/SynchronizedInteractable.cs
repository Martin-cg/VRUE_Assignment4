using Assets.Resources.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(XRBaseInteractable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonRigidbodyView))]
public class SynchronizedInteractable : MonoBehaviourPun {
    protected XRBaseInteractable Interactable;

    protected virtual void Reset() => Init();
    protected virtual void OnValidate() => Init();
    protected virtual void Awake() => Init();

    private void Init() {
        Interactable = Interactable == null ? GetComponent<XRBaseInteractable>() : Interactable;
        photonView.OwnershipTransfer = OwnershipOption.Takeover;
    }

    protected virtual void OnEnable() {
        Interactable.selectEntered.AddListener(OnSelectEntered);
    }
    protected virtual void OnDisable() {
        Interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args) {
        if (args.interactorObject.transform.GetComponentInParent<XRRig>() == null) {
            return;
        }

        photonView.RequestOwnership();
        photonView.RPC(nameof(ForceDropInteractable), RpcTarget.Others);
        // RequestOwnership does not update IsMine immediatly, which confuses our other scripts for a few frames.
        typeof(PhotonView).GetProperty(nameof(photonView.IsMine)).SetValue(photonView, true);
        Debug.Assert(photonView.IsMine);
    }

    [PunRPC]
    private void ForceDropInteractable() {
        foreach (var interactor in Interactable.interactorsSelecting.ToArray()) {
            if (LocalCharacter.Instance.Rig.transform.IsParentOf(interactor.transform)) {
                if (Interactable is CustomXRGrabInteractable custom) {
                    custom.BlockInteractorFor(interactor, 1);
                }

                Interactable.interactionManager.SelectExit(interactor, Interactable);
            }
        }
    }
}
