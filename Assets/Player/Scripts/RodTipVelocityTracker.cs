using UnityEngine;

public class RodTipVelocityTracker : MonoBehaviour
{
    [Header("Velocity")]
    [SerializeField] private float smoothing = 14f;

    public Vector3 RawVelocity { get; private set; }
    public Vector3 SmoothedVelocity { get; private set; }
    public float Speed => SmoothedVelocity.magnitude;

    private Vector3 previousPosition;

    private void Awake()
    {
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        RawVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;

        SmoothedVelocity = Vector3.Lerp(
            SmoothedVelocity,
            RawVelocity,
            1f - Mathf.Exp(-smoothing * Time.fixedDeltaTime)
        );

        previousPosition = transform.position;
    }
}