using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MasterClientOnly : MonoBehaviourPunCallbacks {
    public List<Behaviour> TargetBehaviours = new();
    private bool AreTargetsEnabled;

    protected virtual void Awake() {
        SetTargetsState(false);
    }

    protected virtual void Reset() {
        SetTargetsState(false);
    }

    private void SetTargetsState(bool enabled) {
        foreach (var behaviour in TargetBehaviours) {
            behaviour.enabled = enabled;
        }
        AreTargetsEnabled = enabled;
    }
    private void UpdateTargetsState() {
        if (AreTargetsEnabled == PhotonNetwork.IsMasterClient) {
            return;
        }

        SetTargetsState(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        UpdateTargetsState();
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        base.OnMasterClientSwitched(newMasterClient);

        UpdateTargetsState();
    }
}
