using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class DashLocomotionProvider : ContinuousMoveProviderBase {
    public GameObject LeftHand;
    public GameObject RightHand;
    public float Duration = 0.33f;
    public float CurveExponent = 3;

    private const int PreviousPositionCount = 2;
    private readonly Vector3[] PreviousLeftPositions = new Vector3[PreviousPositionCount];
    private readonly Vector3[] PreviousRightPositions = new Vector3[PreviousPositionCount];

    public bool IsDashing { get; private set; }
    private float DashTime;
    private float DashModifier;

    protected virtual void Start() {
        ResetPositions();
    }

    protected virtual void Reset() {
        system = system == null ? GetComponent<LocomotionSystem>() : system;
    }

    private void ResetPositions() {
        for (int i = 0; i < PreviousPositionCount; i++) {
            PreviousLeftPositions[i] = LeftHand.transform.position;
            PreviousRightPositions[i] = RightHand.transform.position;
        }
    }

    protected new void Update() {
        if (IsDashing) {
            DashTime += Time.deltaTime * (1/DashModifier);
            if (DashTime > Duration) {
                IsDashing = false;
                DashTime = 0;
            }
        }

        base.Update();
    }

    protected override Vector2 ReadInput() {
        for (int i = 1; i < PreviousPositionCount; i++) {
            PreviousLeftPositions[i] = PreviousLeftPositions[i - 1];
            PreviousRightPositions[i] = PreviousRightPositions[i - 1];
        }
        PreviousLeftPositions[0] = LeftHand.transform.position;
        PreviousRightPositions[0] = RightHand.transform.position;

        if (IsDashing) {
            return GetCurrentDashVelocity();
        }

        var leftVelocity = PreviousLeftPositions[0] - PreviousLeftPositions[PreviousPositionCount - 1];
        var rightVelocity = PreviousRightPositions[0] - PreviousRightPositions[PreviousPositionCount - 1];

        var leftMagnitude = leftVelocity.magnitude;
        var rightMagnitude = rightVelocity.magnitude;

        // Ignore small movements
        if (Mathf.Max(leftMagnitude, rightMagnitude) < 0.01 * PreviousPositionCount) {
            return Vector2.zero;
        }

        var forward = forwardSource.transform.forward;
        forward = Vector3.ProjectOnPlane(forward, transform.up);

        var dotProductLeft = Vector3.Dot(forward, leftVelocity.normalized);
        var dotProductRight = Vector3.Dot(forward, rightVelocity.normalized);

        // Ignore movement forward
        if (dotProductLeft > 0 || dotProductRight > 0) {
            return Vector2.zero;
        }

        var leftDirectedMagnitude = -Vector3.Dot(forward, leftVelocity);
        var rightDirectedMagnitude = -Vector3.Dot(forward, rightVelocity);

        // Ignore movement of only one hand
        if (Mathf.Min(leftDirectedMagnitude, rightDirectedMagnitude)*2 < Mathf.Max(leftDirectedMagnitude, rightDirectedMagnitude)) {
            return Vector2.zero;
        }

        // Ignore slow movement
        var combinedDirectedMagnitude = leftDirectedMagnitude * rightDirectedMagnitude;
        var combinedDirectedMagnitudeThreshold = 0.0006f;
        if (combinedDirectedMagnitude < combinedDirectedMagnitudeThreshold) {
            return Vector2.zero;
        }

        IsDashing = true;
        DashModifier = Mathf.Clamp(combinedDirectedMagnitude/combinedDirectedMagnitudeThreshold, 1, 2) - 0.5f;
        DashTime = 0;

        ResetPositions();

        return GetCurrentDashVelocity(); 
    }

    private Vector2 GetCurrentDashVelocity() {
        var magnitude = EaseOutExpo(DashTime / Duration);
        return new Vector2(0, -magnitude); // This weirdness is left as an exercise for the reader
    }

    private float EaseOutExpo(float x) {
        return 1 - Mathf.Pow(CurveExponent, -10 * (x-1));
    }
}
