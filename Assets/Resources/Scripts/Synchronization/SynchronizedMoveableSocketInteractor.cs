using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(XRBaseInteractable))]
public class SynchronizedMoveableSocketInteractor : MonoBehaviourPun, IOnPhotonViewControllerChange {
    protected XRBaseInteractable Interactable;
    protected XRSocketInteractor Socket;

    protected virtual void Awake() => EstablishReferences();

    private void EstablishReferences() {
        Interactable = GetComponent<XRBaseInteractable>();
        Socket = GetComponentInChildren<XRSocketInteractor>();
    }

    public void OnControllerChange(Player newOwner, Player previousOwner) {
        foreach (var interactable in Socket.interactablesSelected) {
            var photonView = PhotonView.Get(interactable.transform);
            photonView.RequestOwnership();
        }
    }
}
