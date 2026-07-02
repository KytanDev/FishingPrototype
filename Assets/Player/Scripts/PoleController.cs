using UnityEngine;
using UnityEngine.InputSystem;

public class PoleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform polePivot;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;

    [Header("Rig Data")]
    [SerializeField] private PoleRigData rigData;

    [Header("Control")]
    [SerializeField] private float yawSpeed = 0.1f;
    [SerializeField] private float pitchSpeed = 0.1f;
    [SerializeField] private float followSpeed = 20f;
    [SerializeField] private float inputDeadzone = 0.01f;

    [Header("Pole Arc Limits")]
    [SerializeField] private float minYaw = -80f;
    [SerializeField] private float maxYaw = 80f;
    [SerializeField] private float minPitch = -10f;
    [SerializeField] private float maxPitch = 85f;

    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    public PoleRigData RigData => rigData;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = 0f;
        pitch = 20f;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!PlayerController.RodControlMode)
        {
            lookInput = Vector2.zero;
            return;
        }

        if (context.performed)
            lookInput = context.ReadValue<Vector2>();

        if (context.canceled)
            lookInput = Vector2.zero;
    }

    public void ClearLookInput()
    {
        lookInput = Vector2.zero;
    }

    private void Update()
    {
        AimArmAt(leftArm, leftHandTarget);
        AimArmAt(rightArm, rightHandTarget);

        if (!PlayerController.RodControlMode)
        {
            polePivot.localRotation = Quaternion.Slerp(
                polePivot.localRotation,
                Quaternion.Euler(pitch, 0f, 0f),
                followSpeed * Time.deltaTime
            );

            yaw = 0f;
            return;
        }

        if (lookInput.sqrMagnitude >= inputDeadzone)
        {
            yaw += lookInput.x * yawSpeed;
            pitch -= lookInput.y * pitchSpeed;

            yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);

        polePivot.localRotation = Quaternion.Slerp(
            polePivot.localRotation,
            targetRotation,
            followSpeed * Time.deltaTime
        );
    }

    private void AimArmAt(Transform arm, Transform target)
    {
        if (arm == null || target == null)
            return;

        Vector3 direction = target.position - arm.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        arm.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}