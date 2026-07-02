using UnityEngine;

public class GerstnerWater : MonoBehaviour
{
    [System.Serializable]
    public struct GerstnerWave
    {
        public Vector2 direction;
        public float amplitude;
        public float waveLength;
        public float speed;
        public float steepness;
    }

    [Header("Water Surface")]
    [SerializeField] private bool useTopOfColliderAsSurface = true;
    [SerializeField] private Collider waterCollider;

    [Header("Visual Material")]
    [SerializeField] private Renderer waterRenderer;

    [Header("Waves")]
    [SerializeField] private GerstnerWave[] waves;

    private Material waterMaterial;

    private void Awake()
    {
        if (waterRenderer != null)
            waterMaterial = waterRenderer.material;
    }

    private void Update()
    {
        SendWavesToShader();
    }

    private void SendWavesToShader()
    {
        if (waterMaterial == null || waves == null)
            return;

        for (int i = 0; i < Mathf.Min(waves.Length, 4); i++)
        {
            GerstnerWave wave = waves[i];
            Vector2 direction = wave.direction.normalized;

            waterMaterial.SetVector("_Wave" + (i + 1) + "Direction", direction);
            waterMaterial.SetFloat("_Wave" + (i + 1) + "Amplitude", wave.amplitude);
            waterMaterial.SetFloat("_Wave" + (i + 1) + "Length", wave.waveLength);
            waterMaterial.SetFloat("_Wave" + (i + 1) + "Speed", wave.speed);
        }
    }

    public float GetWaveHeight(Vector3 worldPosition)
    {
        float height = GetBaseWaterHeight();

        for (int i = 0; i < waves.Length; i++)
        {
            GerstnerWave wave = waves[i];

            if (wave.waveLength <= 0f)
                continue;

            Vector2 direction = wave.direction.normalized;

            float k = 2f * Mathf.PI / wave.waveLength;
            float phase = k * Vector2.Dot(direction, new Vector2(worldPosition.x, worldPosition.z));
            float time = Time.time * wave.speed;

            height += Mathf.Sin(phase + time) * wave.amplitude;
        }

        return height;
    }

    private float GetBaseWaterHeight()
    {
        if (useTopOfColliderAsSurface && waterCollider != null)
            return waterCollider.bounds.max.y;

        return transform.position.y;
    }
}