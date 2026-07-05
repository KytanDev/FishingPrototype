using UnityEngine;

public class RodControlController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform rodPivot;

    [Header("Rod Control")]
    [SerializeField] private float yawSpeed = 120f;
    [SerializeField] private float pitchSpeed = 90f;

    [SerializeField] private float minPitch = -65f;
    [SerializeField] private float maxPitch = 75f;

    [Header("Physics Feel")]
    [SerializeField] private float rodWeightResistance = 0.15f;
    [SerializeField] private float returnToCenterForce = 2f;

    private float rodYaw;
    private float rodPitch;

    private Vector2 rodAngularVelocity;

    public Vector3 RodForward => rodPivot.forward;

    private void Update()
    {
        if (!PlayerState.RodControlMode)
            return;

        Vector2 lookInput = input.LookInput;

        rodAngularVelocity += lookInput * Time.deltaTime;

        rodAngularVelocity = Vector2.Lerp(
            rodAngularVelocity,
            Vector2.zero,
            rodWeightResistance * Time.deltaTime
        );

        rodYaw += lookInput.x * yawSpeed * Time.deltaTime;
        rodPitch -= lookInput.y * pitchSpeed * Time.deltaTime;

        rodPitch = Mathf.Clamp(rodPitch, minPitch, maxPitch);

        ApplyRodRotation();
    }

    private void ApplyRodRotation()
    {
        Quaternion cameraSpace = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);

        Quaternion localRodRotation =
            Quaternion.Euler(rodPitch, rodYaw, 0f);

        rodPivot.rotation = cameraSpace * localRodRotation;
    }
}