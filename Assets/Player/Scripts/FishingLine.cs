using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    public enum LineState
    {
        Held,
        Free
    }

    [Header("Rope")]
    [SerializeField] private int numOfLineSegments = 24;

    [Header("Line Length")]
    [SerializeField] private float releasedLineLength = 2f;
    [SerializeField] private float maxLineLength = 25f;
    [SerializeField] private float minLineLength = 0.5f;
    [SerializeField] private float feedSpeed = 35f;

    [Header("Verlet Physics")]
    [SerializeField] private Vector3 gravityForce = new Vector3(0f, -9.81f, 0f);
    [SerializeField] private float dampingFactor = 0.985f;
    [SerializeField] private int numOfConstraintRuns = 16;

    [Header("Tension")]
    [SerializeField] private float tensionStrength = 8f;
    [SerializeField] private float tensionDamping = 1.5f;
    [SerializeField] private float maxTensionForce = 20f;
    [SerializeField] private float tautTolerance = 0.03f;

    [Header("References")]
    [SerializeField] private Rigidbody rodTip;
    [SerializeField] private Rigidbody floatBody;

    public LineState State { get; private set; } = LineState.Held;

    public float ReleasedLineLength => releasedLineLength;
    public float MaxLineLength => maxLineLength;
    public float DistanceNeeded { get; private set; }
    public float SlackAmount { get; private set; }
    public float StretchAmount { get; private set; }
    public bool IsTaut => StretchAmount > tautTolerance;

    private LineRenderer lineRenderer;
    private readonly List<LineSegment> lineSegments = new List<LineSegment>();

    private float SegmentLength
    {
        get
        {
            if (numOfLineSegments <= 1)
                return releasedLineLength;

            return releasedLineLength / (numOfLineSegments - 1);
        }
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        numOfLineSegments = Mathf.Max(2, numOfLineSegments);
        lineRenderer.positionCount = numOfLineSegments;

        releasedLineLength = Mathf.Clamp(releasedLineLength, minLineLength, maxLineLength);

        CreateLine();
    }

    private void FixedUpdate()
    {
        if (rodTip == null || floatBody == null)
            return;

        UpdateLineState();
        Simulate();

        for (int i = 0; i < numOfConstraintRuns; i++)
        {
            ApplyConstraints();
        }

        ApplyTensionToFloat();
    }

    private void LateUpdate()
    {
        DrawLine();
    }

    private void CreateLine()
    {
        lineSegments.Clear();

        Vector3 start = rodTip != null ? rodTip.position : transform.position;
        Vector3 direction = Vector3.down;

        float segmentLength = SegmentLength;

        for (int i = 0; i < numOfLineSegments; i++)
        {
            Vector3 position = start + direction * segmentLength * i;
            lineSegments.Add(new LineSegment(position));
        }
    }

    private void UpdateLineState()
    {
        DistanceNeeded = Vector3.Distance(rodTip.position, floatBody.position);

        if (State == LineState.Free)
        {
            // If the float moves farther away than the line currently out,
            // let more line peel off the reel.
            if (DistanceNeeded > releasedLineLength)
            {
                releasedLineLength = Mathf.MoveTowards(
                    releasedLineLength,
                    DistanceNeeded,
                    feedSpeed * Time.fixedDeltaTime
                );
            }
        }

        releasedLineLength = Mathf.Clamp(releasedLineLength, minLineLength, maxLineLength);

        SlackAmount = Mathf.Max(0f, releasedLineLength - DistanceNeeded);
        StretchAmount = Mathf.Max(0f, DistanceNeeded - releasedLineLength);
    }

    private void Simulate()
    {
        // Skip first and last because they are pinned to rod tip and float.
        for (int i = 1; i < lineSegments.Count - 1; i++)
        {
            LineSegment segment = lineSegments[i];

            Vector3 velocity = (segment.currentPosition - segment.oldPosition) * dampingFactor;

            segment.oldPosition = segment.currentPosition;
            segment.currentPosition += velocity;
            segment.currentPosition += gravityForce * Time.fixedDeltaTime * Time.fixedDeltaTime;

            lineSegments[i] = segment;
        }
    }

    private void ApplyConstraints()
    {
        float segmentLength = SegmentLength;

        // Keep first point attached to rod tip
        LineSegment firstSegment = lineSegments[0];
        firstSegment.currentPosition = rodTip.position;
        lineSegments[0] = firstSegment;

        // Keep last point attached to float visually
        LineSegment lastSegment = lineSegments[lineSegments.Count - 1];
        lastSegment.currentPosition = floatBody.position;
        lineSegments[lineSegments.Count - 1] = lastSegment;

        for (int i = 0; i < lineSegments.Count - 1; i++)
        {
            LineSegment currentSeg = lineSegments[i];
            LineSegment nextSeg = lineSegments[i + 1];

            Vector3 delta = currentSeg.currentPosition - nextSeg.currentPosition;
            float distance = delta.magnitude;

            if (distance <= 0.0001f)
                continue;

            float difference = distance - segmentLength;
            Vector3 changeDirection = delta / distance;
            Vector3 changeVector = changeDirection * difference;

            bool currentIsPinned = i == 0;
            bool nextIsPinned = i + 1 == lineSegments.Count - 1;

            if (currentIsPinned && !nextIsPinned)
            {
                nextSeg.currentPosition += changeVector;
            }
            else if (!currentIsPinned && nextIsPinned)
            {
                currentSeg.currentPosition -= changeVector;
            }
            else if (!currentIsPinned && !nextIsPinned)
            {
                currentSeg.currentPosition -= changeVector * 0.5f;
                nextSeg.currentPosition += changeVector * 0.5f;
            }

            lineSegments[i] = currentSeg;
            lineSegments[i + 1] = nextSeg;
        }
    }

    private void ApplyTensionToFloat()
    {
        if (StretchAmount <= tautTolerance)
            return;

        Vector3 rodToFloat = floatBody.position - rodTip.position;
        float distance = rodToFloat.magnitude;

        if (distance <= 0.0001f)
            return;

        Vector3 directionFromRodToFloat = rodToFloat / distance;
        Vector3 pullDirection = -directionFromRodToFloat;

        Vector3 relativeVelocity = floatBody.linearVelocity - rodTip.linearVelocity;
        float separatingSpeed = Vector3.Dot(relativeVelocity, directionFromRodToFloat);

        float forceAmount = StretchAmount * tensionStrength;

        if (separatingSpeed > 0f)
            forceAmount += separatingSpeed * tensionDamping;

        forceAmount = Mathf.Clamp(forceAmount, 0f, maxTensionForce);

        floatBody.AddForce(pullDirection * forceAmount, ForceMode.Force);
    }

    private void DrawLine()
    {
        if (lineRenderer == null || lineSegments.Count == 0)
            return;

        lineRenderer.positionCount = lineSegments.Count;

        for (int i = 0; i < lineSegments.Count; i++)
        {
            lineRenderer.SetPosition(i, lineSegments[i].currentPosition);
        }
    }

    public void SetMaxLength(float value)
    {
        maxLineLength = Mathf.Max(minLineLength, value);
        releasedLineLength = Mathf.Clamp(releasedLineLength, minLineLength, maxLineLength);
    }

    public void SetReleasedLength(float value)
    {
        float safeMinimum = 0f;

        if (rodTip != null && floatBody != null)
            safeMinimum = Vector3.Distance(rodTip.position, floatBody.position) - 0.15f;

        releasedLineLength = Mathf.Clamp(
            value,
            Mathf.Max(minLineLength, safeMinimum),
            maxLineLength
        );
    }

    public void HoldLine()
    {
        State = LineState.Held;
    }

    public void ReleaseForCast()
    {
        State = LineState.Free;
    }

    public struct LineSegment
    {
        public Vector3 currentPosition;
        public Vector3 oldPosition;

        public LineSegment(Vector3 pos)
        {
            currentPosition = pos;
            oldPosition = pos;
        }
    }
}