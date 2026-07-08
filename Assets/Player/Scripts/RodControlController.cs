using UnityEngine;

public class RodControlController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Transform rodPitchPivot;

    [Header("Mouse To Rod Axis")]
    [SerializeField] private float cursorSensitivity = 0.0025f;

    [Header("Rod Limits")]
    [SerializeField] private float maxYaw = 65f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 75f;

    private Vector2 rodAxis;

    private void Update()
    {
        if (!PlayerState.RodControlMode)
            return;

        Vector2 look = input.LookInput;

        // Left/right mouse = left/right rod yaw
        rodAxis.x += look.x * cursorSensitivity;

        // Mouse DOWN raises rod, mouse UP lowers rod
        rodAxis.y -= look.y * cursorSensitivity;

        rodAxis.x = Mathf.Clamp(rodAxis.x, -1f, 1f);
        rodAxis.y = Mathf.Clamp(rodAxis.y, -1f, 1f);

        float yaw = rodAxis.x * maxYaw;
        float pitch = Mathf.Lerp(minPitch, maxPitch, (rodAxis.y + 1f) * 0.5f);

        // Yaw only. No roll.
        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

        // Pitch only. No roll.
        if (rodPitchPivot != null)
            rodPitchPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}