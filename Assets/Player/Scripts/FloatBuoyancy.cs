using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class FloatBuoyancy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GerstnerWater water;

    [Header("Buoyancy")]
    [SerializeField] private float maxBuoyancyForce = 0.7f;

    [Header("Water Damping")]
    [SerializeField] private float verticalDamping = 1.8f;
    [SerializeField] private float horizontalDamping = 0.5f;

    [Header("Debug")]
    [SerializeField] private float waterHeight;
    [SerializeField] private float sphereBottomY;
    [SerializeField] private float sphereTopY;
    [SerializeField] private float submergedPercent;
    [SerializeField] private float appliedBuoyancyForce;

    private Rigidbody rb;
    private SphereCollider sphere;
    private float radius;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphere = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        radius = sphere.radius * transform.lossyScale.y;

        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void FixedUpdate()
    {
        if (water == null)
            return;

        ApplyBuoyancy();
        ApplyWaterDamping();
        DrawDebugLines();
    }

    private void ApplyBuoyancy()
    {
        Vector3 center = transform.TransformPoint(sphere.center);

        waterHeight = water.GetWaveHeight(center);

        sphereBottomY = center.y - radius;
        sphereTopY = center.y + radius;

        submergedPercent = CalculateSubmergedPercent();

        if (submergedPercent <= 0f)
        {
            appliedBuoyancyForce = 0f;
            return;
        }

        appliedBuoyancyForce = maxBuoyancyForce * submergedPercent;

        rb.AddForceAtPosition(
            Vector3.up * appliedBuoyancyForce,
            center,
            ForceMode.Force
        );
    }

    private float CalculateSubmergedPercent()
    {
        if (waterHeight <= sphereBottomY)
            return 0f;

        if (waterHeight >= sphereTopY)
            return 1f;

        float submergedDepth = waterHeight - sphereBottomY;
        submergedDepth = Mathf.Clamp(submergedDepth, 0f, radius * 2f);

        float h = submergedDepth;
        float r = radius;

        float submergedVolume =
            Mathf.PI * h * h * (3f * r - h) / 3f;

        float totalVolume =
            4f * Mathf.PI * r * r * r / 3f;

        return Mathf.Clamp01(submergedVolume / totalVolume);
    }

    private void ApplyWaterDamping()
    {
        if (submergedPercent <= 0f)
            return;

        Vector3 velocity = rb.linearVelocity;

        Vector3 verticalVelocity = Vector3.up * velocity.y;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        rb.AddForce(
            -verticalVelocity * verticalDamping * submergedPercent,
            ForceMode.Force
        );

        rb.AddForce(
            -horizontalVelocity * horizontalDamping * submergedPercent,
            ForceMode.Force
        );
    }

    private void DrawDebugLines()
    {
        Vector3 center = transform.TransformPoint(sphere.center);

        Debug.DrawLine(
            new Vector3(center.x - 0.5f, waterHeight, center.z),
            new Vector3(center.x + 0.5f, waterHeight, center.z),
            Color.cyan
        );

        Debug.DrawLine(
            new Vector3(center.x, sphereBottomY, center.z),
            new Vector3(center.x, sphereTopY, center.z),
            Color.yellow
        );
    }
}