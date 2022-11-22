using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ArmSwingMovement : MonoBehaviour {
    public Camera PlayerCamera;

    public GameObject LeftHand, RightHand;

    private CharacterController CharController;
    private Vector3 PrevPosLeft, PrevPosRight;

    private const float AccelerationPerSwing = 0.25f;
    private const float Decceleration = 0.1f;
    private const float MaxVelocity = 4.0F;
    private const float MaxPenaltyVelocity = MaxVelocity / 4;
    private const float PenaltyTime = 5; // secs
    private float CurrentMaxVelocity = MaxVelocity;
    private float LastPenaltyTime = 0;

    private float Acceleration = 0.0F;
    private float Velocity = 0.0F;

    // Start is called before the first frame update
    void Start() {
        CharController = GetComponent<CharacterController>();

        PrevPosLeft = LeftHand.transform.position;
        PrevPosRight = RightHand.transform.position;
    }

    // Update is called once per frame
    void Update() {
        Vector3 leftVelocity = LeftHand.transform.localPosition - PrevPosLeft;
        Vector3 rightVelocity =  RightHand.transform.localPosition - PrevPosRight;

        if (PlayerCamera) {
            float dotProductLeft = Math.Abs(Vector3.Dot(Vector3.up, leftVelocity.normalized));
            float dotProductRight = Math.Abs(Vector3.Dot(Vector3.up, rightVelocity.normalized));

            float combined = dotProductLeft * dotProductRight;
            if (Vector3.Dot(leftVelocity.normalized, rightVelocity.normalized) > 0) {
                combined = 0;
            }

            float velMag = leftVelocity.magnitude + rightVelocity.magnitude;

            float swingStrength = combined * velMag;

            if (swingStrength >= 0.075F) {
                Acceleration += AccelerationPerSwing;
                // Debug.Log($"{dotProductLeft} * {dotProductRight} = {combined}");
                // Debug.Log($"{velMag} * {combined} = {swingStrength}");
            }

            if (Velocity >= 0) {
                Vector3 forwardDirProjected = Vector3.ProjectOnPlane(PlayerCamera.transform.forward, Vector3.up);
                CharController.Move(forwardDirProjected.normalized * Velocity * Time.deltaTime);
            }
        }

        CharController.Move(Physics.gravity * Time.deltaTime);

        CurrentMaxVelocity = Mathf.Lerp(MaxPenaltyVelocity, MaxVelocity, EaseOutExpo((Time.time - LastPenaltyTime) / PenaltyTime));

        Velocity += Acceleration;
        Velocity -= Decceleration;
        Velocity = Math.Clamp(Velocity, 0.0F, CurrentMaxVelocity);
        Acceleration = 0.0F;

        PrevPosLeft = LeftHand.transform.localPosition;
        PrevPosRight = RightHand.transform.localPosition;
    }

    private float EaseOutExpo(float x) {
        x = Mathf.Clamp01(x);
        return 1 - (1 - x) * (1 - x);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (!hit.collider.CompareTag(Tags.Obstacle)) {
            return;
        }
        Debug.Log(hit.collider);

        CurrentMaxVelocity = MaxVelocity / 2;
        LastPenaltyTime = Time.time;
    }

    private Vector3 Abs(Vector3 v) {
        return new Vector3(
            Math.Abs(v.x),
            Math.Abs(v.y),
            Math.Abs(v.z)
        );
    }
}
