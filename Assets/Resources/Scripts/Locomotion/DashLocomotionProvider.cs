using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class DashLocomotionProvider : ContinuousMoveProviderBase {
    public GameObject LeftHand;
    public GameObject RightHand;
    public float Duration = 0.33f;
    public float CurveExponent = 3;

    private const int PreviousPositionCount = 2;
    private Vector3[] PreviousLeftPositions = new Vector3[PreviousPositionCount];
    private Vector3[] PreviousRightPositions = new Vector3[PreviousPositionCount];

    public bool IsDashing { get; private set; }
    private float DashTime;
    private float DashModifier;

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
        var combined = dotProductLeft * dotProductRight;

        // Ignore movement forward
        if (dotProductLeft > 0 || dotProductRight > 0) {
            return Vector2.zero;
        }

        var leftDirectedMagnitude = -Vector3.Dot(forward, leftVelocity);
        var rightDirectedMagnitude = -Vector3.Dot(forward, rightVelocity);

        // Ignore movement of only one hand
        Debug.Log($"{(Mathf.Min(leftDirectedMagnitude, rightDirectedMagnitude) * 2):0.00000} {(Mathf.Max(leftDirectedMagnitude, rightDirectedMagnitude)):0.00000}");
        if (Mathf.Min(leftDirectedMagnitude, rightDirectedMagnitude)*2 < Mathf.Max(leftDirectedMagnitude, rightDirectedMagnitude)) {
            return Vector2.zero;
        }

        // Ignore slow movement
        var combinedDirectedMagnitude = leftDirectedMagnitude * rightDirectedMagnitude;
        Debug.Log($"{combinedDirectedMagnitude:0.00000}");
        var combinedDirectedMagnitudeThreshold = 0.0006f;
        if (combinedDirectedMagnitude < combinedDirectedMagnitudeThreshold) {
            return Vector2.zero;
        }

        // Debug.Log($"{dotProductLeft:0.00000} {dotProductRight:0.00000} {combined:0.00000}");

        // var swingStrength = combined * (leftMagnitude + rightMagnitude);
        // if (swingStrength >= 0.1 * PreviousPositionCount) {
        IsDashing = true;
        DashModifier = Mathf.Clamp(combinedDirectedMagnitude/combinedDirectedMagnitudeThreshold, 1, 2) - 0.5f;
        // DashModifier = EaseDashModifier(combinedDirectedMagnitude, combinedDirectedMagnitudeThreshold, combinedDirectedMagnitudeThreshold*2, 0.5f, 2);
        DashTime = 0;

        Debug.LogWarning("DASH!");
        ResetPositions();
        // }

        return GetCurrentDashVelocity(); 
    }

    private Vector2 GetCurrentDashVelocity() {
        var magnitude = EaseOutExpo(DashTime / Duration);
        return new Vector2(0, -magnitude); // This weirdness is left as an exercise for the reader
    }

    private float EaseOutExpo(float x) {
        return 1 - Mathf.Pow(CurveExponent, -10 * (x-1));
    }

    private float EaseDashModifier(float swing, float minTarget, float maxTarget, float minMod, float maxMod) {
        // https://www.desmos.com/calculator/004ai7no3p
        float t = (swing - minTarget) / maxTarget;
        t = Mathf.Clamp01(t);
        return 1.5f - Mathf.Pow(1.5f - minMod - (maxMod - minMod) * t, 2);
    }
}
