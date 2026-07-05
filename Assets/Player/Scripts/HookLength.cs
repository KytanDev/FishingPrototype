using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HookLength : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody floatObj;
    [SerializeField] private Rigidbody hookObj;

    [Header("Line Settings")]
    [SerializeField] private float maxLength = 1.0f;

    [Header("Tension Physics")]
    [SerializeField] private float stiffness = 0.8f;
    [SerializeField] private float damping = 0.08f;
    [SerializeField] private float maxTensionForce = 0.08f;

    [Header("Renderer")]
    [SerializeField] private float lineWidth = 0.01f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true;

    }

    private void FixedUpdate()
    {
        if (floatObj == null || hookObj == null) return;

        ApplyLineConstraint();
    }

    private void LateUpdate()
    {
        DrawLine();
    }

    private void ApplyLineConstraint()
    {
        Vector3 floatPos = floatObj.worldCenterOfMass;
        Vector3 hookPos = hookObj.worldCenterOfMass;

        Vector3 difference = hookPos - floatPos;
        float distance = difference.magnitude;

        if (distance <= maxLength) return;

        Vector3 direction = difference.normalized;
        float stretchDistance = distance - maxLength;

        Vector3 relativeVelocity = hookObj.linearVelocity - floatObj.linearVelocity;
        float velocityAlongLine = Vector3.Dot(relativeVelocity, direction);

        float tensionForce = (stretchDistance * stiffness) + (velocityAlongLine * damping);
        tensionForce = Mathf.Clamp(tensionForce, 0f, maxTensionForce);

        Vector3 force = direction * tensionForce;
        floatObj.AddForce(force, ForceMode.Force);
        hookObj.AddForce(-force, ForceMode.Force);
    }

    private void DrawLine()
    {
        if (lineRenderer == null || floatObj == null || hookObj == null) return;

        lineRenderer.SetPosition(0, floatObj.worldCenterOfMass);
        lineRenderer.SetPosition(1, hookObj.worldCenterOfMass);
    }
}
