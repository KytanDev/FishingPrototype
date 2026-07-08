using UnityEngine;

public class RodTipCastController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private RodTipVelocityTracker tipVelocity;
    [SerializeField] private Reel reel;
    [SerializeField] private FishingLine mainLine;
    [SerializeField] private Rigidbody castBody;

    [Header("Cast Physics")]
    [SerializeField] private float minimumTipSpeed = 2.5f;
    [SerializeField] private float castVelocityMultiplier = 1.15f;
    [SerializeField] private float forwardBias = 3f;
    [SerializeField] private float upwardBias = 1.2f;
    [SerializeField] private float maxCastSpeed = 22f;

    [Header("Direction Filtering")]
    [SerializeField] private float minUpwardDirection = -0.1f;

    private bool wasHoldingLine;

    private void Start()
    {
        if (reel != null && mainLine != null)
        {
            mainLine.SetMaxLength(reel.MaxLineLength);
            mainLine.SetReleasedLength(reel.ReleasedLineLength);
        }
    }

    private void FixedUpdate()
    {
        if (input == null || mainLine == null || castBody == null || tipVelocity == null)
            return;

        HandleReeling();
        HandleLineInput();
    }

    private void HandleReeling()
    {
        if (reel == null)
            return;

        if (input.ReelInput <= 0f)
            return;

        reel.ReelIn(input.ReelInput);

        mainLine.SetReleasedLength(reel.ReleasedLineLength);
        mainLine.HoldLine();
    }

    private void HandleLineInput()
    {
        if (input.ReelInput > 0f)
            return;

        // Holding trigger = finger on line.
        if (input.HoldLine)
        {
            wasHoldingLine = true;
            mainLine.HoldLine();
            return;
        }

        // Releasing trigger = let line peel off.
        if (wasHoldingLine)
        {
            wasHoldingLine = false;
            ReleaseAndCast();
        }
    }

    private void ReleaseAndCast()
    {
        mainLine.ReleaseForCast();

        Vector3 tipVel = tipVelocity.SmoothedVelocity;
        float tipSpeed = tipVel.magnitude;

        if (tipSpeed < minimumTipSpeed)
            return;

        Vector3 castDirection = tipVel.normalized;

        castDirection += transform.forward * 0.35f;
        castDirection += Vector3.up * 0.12f;

        if (castDirection.y < minUpwardDirection)
            castDirection.y = minUpwardDirection;

        castDirection.Normalize();

        float castSpeed = tipSpeed * castVelocityMultiplier;
        castSpeed += forwardBias;
        castSpeed += upwardBias;
        castSpeed = Mathf.Clamp(castSpeed, 0f, maxCastSpeed);

        Vector3 finalVelocity = castDirection * castSpeed;

        castBody.linearVelocity *= 0.25f;
        castBody.angularVelocity *= 0.25f;

        castBody.AddForce(finalVelocity * castBody.mass, ForceMode.Impulse);
    }
}