using UnityEngine;

public class Reel : MonoBehaviour
{
    [Header("Line Capacity")]
    [SerializeField] private float maxLineLength = 25f;
    [SerializeField] private float startingReleasedLength = 2f;
    [SerializeField] private float minimumReleasedLength = 0.5f;

    [Header("Reeling")]
    [SerializeField] private float reelInSpeed = 2.5f;

    public float MaxLineLength => maxLineLength;
    public float ReleasedLineLength { get; private set; }
    public float MinimumReleasedLength => minimumReleasedLength;
    public float ReelInSpeed => reelInSpeed;

    public float RemainingLine => maxLineLength - ReleasedLineLength;
    public bool HasLineRemaining => ReleasedLineLength < maxLineLength;
    public bool IsFullyReeledIn => ReleasedLineLength <= minimumReleasedLength + 0.01f;

    private void Awake()
    {
        ReleasedLineLength = Mathf.Clamp(
            startingReleasedLength,
            minimumReleasedLength,
            maxLineLength
        );
    }

    public void SetReleasedLength(float value)
    {
        ReleasedLineLength = Mathf.Clamp(
            value,
            minimumReleasedLength,
            maxLineLength
        );
    }

    public void FeedLineToward(float targetLength, float feedSpeed)
    {
        if (targetLength <= ReleasedLineLength)
            return;

        ReleasedLineLength = Mathf.MoveTowards(
            ReleasedLineLength,
            targetLength,
            feedSpeed * Time.fixedDeltaTime
        );

        ReleasedLineLength = Mathf.Min(ReleasedLineLength, maxLineLength);
    }

    public void ReelIn(float inputAmount)
    {
        if (inputAmount <= 0f)
            return;

        ReleasedLineLength -= reelInSpeed * inputAmount * Time.fixedDeltaTime;
        ReleasedLineLength = Mathf.Max(ReleasedLineLength, minimumReleasedLength);
    }
}