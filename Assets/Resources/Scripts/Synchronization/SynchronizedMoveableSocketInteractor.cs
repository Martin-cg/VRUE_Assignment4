using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(XRBaseInteractable))]
public class SynchronizedMoveableSocketInteractor : MonoBehaviourPun, IOnPhotonViewOwnerChange {
    protected XRBaseInteractable Interactable;
    protected XRSocketInteractor Socket;

    protected virtual void Awake() => EstablishReferences();
    protected virtual void Reset() => EstablishReferences();

    private void EstablishReferences() {
        Interactable = GetComponent<XRBaseInteractable>();
        Socket = GetComponentInChildren<XRSocketInteractor>();
    }

    public void OnOwnerChange(Player newOwner, Player previousOwner) {
        foreach (var interactable in Socket.interactablesSelected) {
            var photonView = PhotonView.Get(interactable.transform);
            photonView.RequestOwnership();
        }
    }
}
