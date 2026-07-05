using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Hook : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float hookMass = 0.006f;

    private Rigidbody rb;
    public Rigidbody RigidBody => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = hookMass;
        rb.useGravity = true;
        rb.linearDamping = 0.05f;
        rb.angularDamping = 0.05f;
    }
}
