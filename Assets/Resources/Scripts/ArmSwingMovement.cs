using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class ArmSwingMovement : ContinuousMoveProviderBase {
    public GameObject LeftHand;
    public GameObject RightHand;

    private Vector3 PrevPosLeft, PrevPosRight;

    private const float AccelerationPerSwing = 0.05f;
    private const float Decceleration = 0.025f;
    private const float MaxVelocity = 1f;
    private const float MaxPenaltyVelocity = 0.25f;
    private const float PenaltyTime = 5; // secs
    private float CurrentMaxVelocity = MaxVelocity;
    private float LastPenaltyTime = 0;

    private float Velocity = 0.0F;

    // Start is called before the first frame update
    void Start() {
        PrevPosLeft = LeftHand.transform.position;
        PrevPosRight = RightHand.transform.position;
    }

    protected override Vector2 ReadInput() {
        Vector3 leftVelocity = LeftHand.transform.localPosition - PrevPosLeft;
        Vector3 rightVelocity = RightHand.transform.localPosition - PrevPosRight;

        float dotProductLeft = Math.Abs(Vector3.Dot(Vector3.up, leftVelocity.normalized));
        float dotProductRight = Math.Abs(Vector3.Dot(Vector3.up, rightVelocity.normalized));

        float combined = dotProductLeft * dotProductRight;
        if (Vector3.Dot(leftVelocity.normalized, rightVelocity.normalized) > 0) {
            combined = 0;
        }

        float velMag = leftVelocity.magnitude + rightVelocity.magnitude;

        float swingStrength = combined * velMag;
        if (swingStrength >= 0.02F) {
            Velocity += AccelerationPerSwing * Mathf.Clamp(swingStrength / 0.075f, 0, 2);
        }

        CurrentMaxVelocity = Mathf.Lerp(MaxPenaltyVelocity, MaxVelocity, EaseOutExpo((Time.time - LastPenaltyTime) / PenaltyTime));

        Velocity -= Decceleration;
        Velocity = Math.Clamp(Velocity, 0.0F, CurrentMaxVelocity);

        PrevPosLeft = LeftHand.transform.localPosition;
        PrevPosRight = RightHand.transform.localPosition;

        Vector2 movement = Velocity <= 0 ? Vector2.zero : new Vector2(0, Velocity); // This weirdness is left as an exercise for the reader
        return movement;
    }

    private float EaseOutExpo(float x) {
        x = Mathf.Clamp01(x);
        return 1 - (1 - x) * (1 - x);
    }
}
