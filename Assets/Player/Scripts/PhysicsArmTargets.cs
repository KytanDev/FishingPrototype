using UnityEngine;

public class PhysicsArmTargets : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform rodYawPivot;
    [SerializeField] private Transform rodPitchPivot;

    [Header("Neutral Local Positions")]
    [SerializeField] private Vector3 leftNeutral = new Vector3(-0.25f, -0.25f, 0.65f);
    [SerializeField] private Vector3 rightNeutral = new Vector3(0.25f, -0.2f, 0.85f);

    [Header("Hand Movement")]
    [SerializeField] private float xRange = 0.35f;
    [SerializeField] private float yRange = 0.45f;
    [SerializeField] private float zRange = 0.35f;
    [SerializeField] private float smooth = 14f;

    [Header("Rod Rotation")]
    [SerializeField] private float maxYaw = 75f;

    // You said lowest is 30 and max is -160.
    [SerializeField] private float lowestPitch = 30f;
    [SerializeField] private float highestPitch = -160f;

    [SerializeField] private float rotationSmooth = 12f;

    [Header("Hand Influence")]
    [SerializeField] private float rightHandPitchInfluence = 0.7f;
    [SerializeField] private float leftHandPitchInfluence = 0.3f;
    [SerializeField] private float rightHandYawInfluence = 0.75f;
    [SerializeField] private float leftHandYawInfluence = 0.25f;

    private Vector3 leftVel;
    private Vector3 rightVel;

    private void FixedUpdate()
    {
        if (!PlayerState.RodControlMode)
            return;

        MoveHands();
        RotateRod();
    }

    private void MoveHands()
    {
        Vector2 left = input.LeftArmInput;
        Vector2 right = input.RightArmInput;

        Vector3 leftTarget =
            leftNeutral +
            new Vector3(
                left.x * xRange,
                left.y * yRange,
                left.y * zRange
            );

        Vector3 rightTarget =
            rightNeutral +
            new Vector3(
                right.x * xRange,
                right.y * yRange,
                right.y * zRange
            );

        leftHandTarget.localPosition = Vector3.SmoothDamp(
            leftHandTarget.localPosition,
            leftTarget,
            ref leftVel,
            1f / smooth
        );

        rightHandTarget.localPosition = Vector3.SmoothDamp(
            rightHandTarget.localPosition,
            rightTarget,
            ref rightVel,
            1f / smooth
        );
    }

    private void RotateRod()
    {
        Vector2 left = input.LeftArmInput;
        Vector2 right = input.RightArmInput;

        float yawInput =
            right.x * rightHandYawInfluence +
            left.x * leftHandYawInfluence;

        float pitchInput =
            right.y * rightHandPitchInfluence +
            left.y * leftHandPitchInfluence;

        yawInput = Mathf.Clamp(yawInput, -1f, 1f);

        // No input should sit at lowestPitch.
        // Pulling input upward moves toward highestPitch.
        float pitch01 = Mathf.InverseLerp(-1f, 1f, pitchInput);
        float pitch = Mathf.Lerp(lowestPitch, highestPitch, pitch01);

        float yaw = yawInput * maxYaw;

        Quaternion targetYaw = Quaternion.Euler(0f, yaw, 0f);
        Quaternion targetPitch = Quaternion.Euler(pitch, 0f, 0f);

        rodYawPivot.localRotation = Quaternion.Slerp(
            rodYawPivot.localRotation,
            targetYaw,
            rotationSmooth * Time.fixedDeltaTime
        );

        rodPitchPivot.localRotation = Quaternion.Slerp(
            rodPitchPivot.localRotation,
            targetPitch,
            rotationSmooth * Time.fixedDeltaTime
        );
    }
}