using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PoleController poleController;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody rb;

    public static bool RodControlMode { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            RodControlMode = !RodControlMode;

            rb.angularVelocity = Vector3.zero;

            if (poleController != null)
                poleController.ClearLookInput();
        }
    }

    private void FixedUpdate()
    {
        if (RodControlMode)
        {
            rb.angularVelocity = Vector3.zero;

            Vector3 lockedVelocity = rb.linearVelocity;
            lockedVelocity.x = 0f;
            lockedVelocity.z = 0f;
            rb.linearVelocity = lockedVelocity;

            return;
        }

        MatchPlayerYawToCamera();
        MovePlayer();
    }

    private void MatchPlayerYawToCamera()
    {
        Vector3 cameraEuler = cameraTransform.eulerAngles;
        Quaternion targetRotation = Quaternion.Euler(0f, cameraEuler.y, 0f);

        rb.MoveRotation(targetRotation);
    }

    private void MovePlayer()
    {
        Vector2 moveInput = input.MoveInput;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        Vector3 targetVelocity = moveDirection * moveSpeed;

        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            rb.linearVelocity.y,
            targetVelocity.z
        );
    }
}