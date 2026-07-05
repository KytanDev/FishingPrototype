using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shot : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float shotMass = 0.025f;

    private Rigidbody rb;

    public Rigidbody Rigidbody => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = shotMass;
        rb.useGravity = true;
        rb.linearDamping = 0.05f;
        rb.angularDamping = 0.05f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
}
