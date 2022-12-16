using UnityEngine;

public class ResetOnGameStateChange : MonoBehaviourGameStateCallbacks {
    public bool ResetPose = true;
    public bool ResetParent = true;

    private Pose DefaultPose;
    private Transform DefaultParent;

    protected virtual void Start() {
        DefaultPose = transform.GetPose();
        DefaultParent = transform.parent;

        if (!ResetPose && !ResetParent) {
            Destroy(this);
        }
    }

    public override void OnGameStateChanged(GameState newState) {
        base.OnGameStateChanged(newState);

        if (ResetParent) {
            transform.SetParent(DefaultParent, true);
        }
        if (ResetPose) {
            transform.SetPose(DefaultPose);
        }
    }
}
