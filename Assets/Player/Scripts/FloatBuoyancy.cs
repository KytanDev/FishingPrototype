using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatBuoyancy : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private FloatWaterState waterState = FloatWaterState.Airborne;

    [Header("References")]
    [SerializeField] private GerstnerWater water;
    [SerializeField] private PoleController poleController;

    [Header("Float Sitting Height")]
    [SerializeField] private float surfaceOffset = 0.25f;

    [Header("Debug")]
    [SerializeField] private float currentWaterHeight;
    [SerializeField] private float currentSubmergedPercent;

    private Rigidbody rb;
    private PoleRigData rigData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigData = poleController.RigData;

        rb.mass = rigData.floatPart.massKg;
        rb.useGravity = true;

        rb.linearDamping = rigData.airLinearDamping;
        rb.angularDamping = rigData.airAngularDamping;
    }

    private void FixedUpdate()
    {
        if (waterState != FloatWaterState.InWater)
            return;

        ApplyBuoyancy();
    }

    private void ApplyBuoyancy()
    {
        if (water == null || rigData == null)
            return;

        currentWaterHeight = water.GetWaveHeight(transform.position) + surfaceOffset;

        float floatBottom = transform.position.y - rigData.floatHeight * 0.5f;
        float submergedDepth = currentWaterHeight - floatBottom;

        currentSubmergedPercent = Mathf.Clamp01(submergedDepth / rigData.floatHeight);

        if (currentSubmergedPercent <= 0f)
            return;

        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 buoyancyForce =
            Vector3.up *
            gravity *
            rb.mass *
            rigData.floatBuoyancyMultiplier *
            currentSubmergedPercent;

        float verticalVelocity = rb.linearVelocity.y;

        Vector3 verticalDampingForce =
            Vector3.down * verticalVelocity * rigData.waterVerticalDamping;

        rb.AddForce(buoyancyForce + verticalDampingForce, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Water"))
            return;

        waterState = FloatWaterState.InWater;

        rb.linearDamping = rigData.waterLinearDamping;
        rb.angularDamping = rigData.waterAngularDamping;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Water"))
            return;

        waterState = FloatWaterState.Airborne;

        rb.linearDamping = rigData.airLinearDamping;
        rb.angularDamping = rigData.airAngularDamping;
    }
}