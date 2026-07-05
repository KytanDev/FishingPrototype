using UnityEngine;

public class PlayerLookController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Transform cameraPivot;

    [Header("Look")]
    [SerializeField] private float sensitivity = 0.08f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private float yaw;
    private float pitch;

    public float Yaw => yaw;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = cameraPivot.localEulerAngles.x;
    }

    private void Update()
    {
        if (PlayerState.RodControlMode)
            return;

        Vector2 lookInput = input.LookInput;

        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}