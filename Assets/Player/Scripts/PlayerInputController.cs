using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public Vector2 LeftArmInput { get; private set; }
    public Vector2 RightArmInput { get; private set; }

    public bool RodModePressed { get; private set; }

    public bool HoldLine { get; private set; }
    public float ReelInput { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnLeftArm(InputAction.CallbackContext context)
    {
        LeftArmInput = context.ReadValue<Vector2>();
    }

    public void OnRightArm(InputAction.CallbackContext context)
    {
        RightArmInput = context.ReadValue<Vector2>();
    }

    public void OnHoldLine(InputAction.CallbackContext context)
    {
        if (context.performed)
            HoldLine = true;

        if (context.canceled)
            HoldLine = false;
    }

    public void OnReel(InputAction.CallbackContext context)
    {
        ReelInput = context.ReadValue<float>();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            RodModePressed = true;
    }

    public void ClearFrameInputs()
    {
        RodModePressed = false;
    }
}