using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool RodModePressed { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
            RodModePressed = true;
    }

    public void ClearFrameInputs()
    {
        RodModePressed = false;
    }
}