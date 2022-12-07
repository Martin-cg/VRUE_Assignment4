using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class InOnlineRoomOnly : MonoBehaviourPunCallbacks {
    public List<Behaviour> TargetBehaviours = new();

    protected virtual void Awake() {
        SetTargetsState(false);
    }

    protected virtual void Reset() {
        SetTargetsState(false);
    }

    protected virtual void OnValidate() {
        SetTargetsState(false);
    }

    private void SetTargetsState(bool enabled) {
        if (!this) {
            return;
        }

        for (int i=0; i<TargetBehaviours.Count; i++) {
            var behaviour = TargetBehaviours[i];
            if (behaviour != null && behaviour.gameObject) {
                behaviour.enabled = enabled;
            } else if (Application.isPlaying) {
                TargetBehaviours.RemoveAt(i);
                i -= 1;
            }
        }
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        SetTargetsState(true);
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();

        SetTargetsState(false);
    }
}
