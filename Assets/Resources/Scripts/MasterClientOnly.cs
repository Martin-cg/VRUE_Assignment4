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
        for (int i = 0; i < TargetBehaviours.Count; i++) {
            var behaviour = TargetBehaviours[i];
            if (behaviour != null && behaviour.gameObject) {
                behaviour.enabled = enabled;
            } else if (Application.isPlaying) {
                TargetBehaviours.RemoveAt(i);
                i -= 1;
            }
        }

        AreTargetsEnabled = enabled;
    }
    private void UpdateTargetsState() {
        var targetState = PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient;

        if (AreTargetsEnabled == targetState) {
            return;
        }

        SetTargetsState(targetState);
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        UpdateTargetsState();
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        base.OnMasterClientSwitched(newMasterClient);

        UpdateTargetsState();
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();

        UpdateTargetsState();
    }
}
