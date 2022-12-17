using UnityEngine;
using UnityEngine.Events;

public class FixedJointEventEmitter : MonoBehaviour {
    public UnityEvent<float> OnJointBroken = new();

    private void OnJointBreak(float breakForce) {
        OnJointBroken.Invoke(breakForce);
        OnJointBroken.RemoveAllListeners();
        Destroy(this);
    }
}
