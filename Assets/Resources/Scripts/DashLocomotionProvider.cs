using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class DashLocomotionProvider : ContinuousMoveProviderBase {
    public GameObject LeftHand;
    public GameObject RightHand;

    private const int PreviousPositionCount = 2;
    private Vector3[] PreviousLeftPositions = new Vector3[PreviousPositionCount];
    private Vector3[] PreviousRightPositions = new Vector3[PreviousPositionCount];

    private float Velocity = 0.0F;

    // Start is called before the first frame update
    void Start() {
        ResetPositions();
    }

    private void Reset() {
        system = system == null ? GetComponent<LocomotionSystem>() : system;
    }

    private void ResetPositions() {
        for (int i = 0; i < PreviousPositionCount; i++) {
            PreviousLeftPositions[i] = LeftHand.transform.position;
            PreviousRightPositions[i] = RightHand.transform.position;
        }
    }

    protected override Vector2 ReadInput() {
        for (int i = 1; i < PreviousPositionCount; i++) {
            PreviousLeftPositions[i] = PreviousLeftPositions[i - 1];
            PreviousRightPositions[i] = PreviousRightPositions[i - 1];
        }
        PreviousLeftPositions[0] = LeftHand.transform.position;
        PreviousRightPositions[0] = RightHand.transform.position;

        var leftVelocity = PreviousLeftPositions[0] - PreviousLeftPositions[PreviousPositionCount - 1];
        var rightVelocity = PreviousRightPositions[0] - PreviousRightPositions[PreviousPositionCount-1];

        var leftMagnitude = leftVelocity.magnitude;
        var rightMagnitude = rightVelocity.magnitude;

        // Ignore small movements
        if (Mathf.Max(leftMagnitude, rightMagnitude) < 0.001 * PreviousPositionCount) {
            return Vector2.zero;
        }

        var forward = forwardSource.transform.forward;
        var dotProductLeft = Vector3.Dot(forward, leftVelocity.normalized);
        var dotProductRight = Vector3.Dot(forward, rightVelocity.normalized);
        var combined = dotProductLeft * dotProductRight;

        // Ignore movement forward
        if (dotProductLeft > 0 || dotProductRight > 0) {
            return Vector2.zero;
        }

        // Ignore movement of only one hand
        // if (Mathf.Max(leftMagnitude, rightMagnitude)*2 > Mathf.Min(leftMagnitude, rightMagnitude)) {
        //     return Vector2.zero;
        // }

        var swingStrength = combined * (leftMagnitude + rightMagnitude);
        if (swingStrength >= 0.1 * PreviousPositionCount) {
            Velocity = 1;
            Debug.LogWarning("DASH!");
            ResetPositions();
        }

        return Velocity <= 0 ? Vector2.zero : new Vector2(0, Velocity); // This weirdness is left as an exercise for the reader
    }
}
