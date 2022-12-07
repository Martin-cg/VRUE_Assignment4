using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnJoin : MonoBehaviourPunCallbacks {
    public List<Behaviour> TargetBehaviours = new();

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
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        SetTargetsState(true);
    }
}
