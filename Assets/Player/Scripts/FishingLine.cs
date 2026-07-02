using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    [SerializeField] private Transform rodTip;
    [SerializeField] private Transform floatTransform;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
    }

    private void LateUpdate()
    {
        line.SetPosition(0, rodTip.position);
        line.SetPosition(1, floatTransform.position);
    }
}