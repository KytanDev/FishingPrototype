using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PoleLineController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rodTip;
    [SerializeField] private Rigidbody floatRb;
    [SerializeField] private PoleController poleController;

    [Header("Line Renderer")]
    [SerializeField] private int linePoints = 12;
    [SerializeField] private float lineSag = 0.25f;

    [Header("Line Safety")]
    [SerializeField] private float stretchClamp = 0.35f;
    [SerializeField] private float tensionMultiplier = 0.65f;
    [SerializeField] private float maxFloatSpeed = 10f;

    private LineRenderer line;
    private PoleRigData rigData;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        rigData = poleController.RigData;

        floatRb.mass = rigData.floatPart.massKg;
        floatRb.useGravity = true;

        line.positionCount = linePoints;
    }

    private void FixedUpdate()
    {
        ApplyLineTension();
        ClampFloatVelocity();
    }

    private void LateUpdate()
    {
        DrawLine();
    }

    private void ApplyLineTension()
    {
        if (rodTip == null || floatRb == null || rigData == null)
            return;

        Vector3 rodTipPosition = rodTip.position;
        Vector3 floatPosition = floatRb.position;

        Vector3 rodToFloat = floatPosition - rodTipPosition;
        float currentDistance = rodToFloat.magnitude;

        if (currentDistance < 0.001f)
            return;

        float maxDistance = rigData.lineLength + rigData.lineSlack;

        if (currentDistance <= maxDistance)
            return;

        Vector3 lineDirection = rodToFloat.normalized;

        float stretchAmount = currentDistance - maxDistance;
        stretchAmount = Mathf.Clamp(stretchAmount, 0f, stretchClamp);

        Vector3 correctionForce =
            -lineDirection *
            stretchAmount *
            rigData.tensionStrength *
            tensionMultiplier;

        float velocityAlongLine = Vector3.Dot(floatRb.linearVelocity, lineDirection);

        Vector3 dampingForce = Vector3.zero;

        if (velocityAlongLine > 0f)
        {
            dampingForce =
                -lineDirection *
                velocityAlongLine *
                rigData.damping *
                tensionMultiplier;
        }

        Vector3 finalForce = correctionForce + dampingForce;

        finalForce = Vector3.ClampMagnitude(finalForce, rigData.maxTensionForce);

        floatRb.AddForce(finalForce, ForceMode.Force);
    }

    private void ClampFloatVelocity()
    {
        if (floatRb == null)
            return;

        floatRb.linearVelocity = Vector3.ClampMagnitude(
            floatRb.linearVelocity,
            maxFloatSpeed
        );
    }

    private void DrawLine()
    {
        if (rodTip == null || floatRb == null)
            return;

        Vector3 start = rodTip.position;
        Vector3 end = floatRb.position;

        line.positionCount = linePoints;

        for (int i = 0; i < linePoints; i++)
        {
            float t = i / (float)(linePoints - 1);

            Vector3 point = Vector3.Lerp(start, end, t);

            float sag = Mathf.Sin(t * Mathf.PI) * lineSag;
            point += Vector3.down * sag;

            line.SetPosition(i, point);
        }
    }
}