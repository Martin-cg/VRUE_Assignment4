using UnityEngine;

public class TransformSync : MonoBehaviour {
    public Transform Target;

    private void Update() {
        if (Target) {
            Target.localPosition = transform.localPosition;
            Target.localRotation = transform.localRotation;
            Target.localScale = transform.localScale;
        }
    }
}
