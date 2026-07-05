using UnityEngine;
using Unity.Cinemachine;

public class CinemachineInputBlocker : MonoBehaviour
{
    private CinemachineInputAxisController inputController;

    private void Awake()
    {
        inputController = GetComponent<CinemachineInputAxisController>();
    }

    private void Update()
    {
        if (inputController == null)
            return;

        inputController.enabled = !PlayerState.RodControlMode;
    }
}