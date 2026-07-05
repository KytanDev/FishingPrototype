using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private PlayerLookController lookController;
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 3f;
    [SerializeField] private float accelerationForce = 35f;
    [SerializeField] private float brakingForce = 45f;
    [SerializeField] private float airControlMultiplier = 0.35f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody rb;

    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        if (input.RodModePressed)
            PlayerState.ToggleRodControlMode();

        input.ClearFrameInputs();
    }

    private void FixedUpdate()
    {
        MatchPlayerYawToLook();

        if (PlayerState.RodControlMode)
        {
            ApplyBraking();
            return;
        }

        ApplyMovementForces();
    }

    private void MatchPlayerYawToLook()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, lookController.Yaw, 0f);
        rb.MoveRotation(targetRotation);
    }

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = true;
            return;
        }

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void ApplyMovementForces()
    {
        Vector2 moveInput = input.MoveInput;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 desiredDirection = forward * moveInput.y + right * moveInput.x;

        if (desiredDirection.sqrMagnitude > 1f)
            desiredDirection.Normalize();

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        Vector3 desiredVelocity = desiredDirection * maxMoveSpeed;
        Vector3 velocityError = desiredVelocity - horizontalVelocity;

        float controlMultiplier = isGrounded ? 1f : airControlMultiplier;

        rb.AddForce(
            velocityError * accelerationForce * controlMultiplier,
            ForceMode.Acceleration
        );

        if (desiredDirection.sqrMagnitude < 0.01f)
            ApplyBraking();
    }

    private void ApplyBraking()
    {
        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        rb.AddForce(
            -horizontalVelocity * brakingForce,
            ForceMode.Acceleration
        );
    }
}