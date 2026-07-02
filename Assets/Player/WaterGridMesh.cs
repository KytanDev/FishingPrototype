using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterGridMesh : MonoBehaviour
{
    [Header("Copy Size From Collider")]
    [SerializeField] private BoxCollider waterCollider;

    [Header("Manual Size")]
    [SerializeField] private float width = 20f;
    [SerializeField] private float length = 20f;

    [Header("Resolution")]
    [SerializeField] private int xSegments = 100;
    [SerializeField] private int zSegments = 100;

    private void Awake()
    {
        GenerateMesh();
    }

    [ContextMenu("Generate Mesh")]
    public void GenerateMesh()
    {
        if (waterCollider != null)
        {
            width = waterCollider.size.x * waterCollider.transform.lossyScale.x;
            length = waterCollider.size.z * waterCollider.transform.lossyScale.z;
        }

        transform.localScale = Vector3.one;

        Mesh mesh = new Mesh();
        mesh.name = "Water Grid";

        int vertCount = (xSegments + 1) * (zSegments + 1);

        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[xSegments * zSegments * 6];

        int v = 0;

        for (int z = 0; z <= zSegments; z++)
        {
            for (int x = 0; x <= xSegments; x++)
            {
                float xPos = ((float)x / xSegments - 0.5f) * width;
                float zPos = ((float)z / zSegments - 0.5f) * length;

                vertices[v] = new Vector3(xPos, 0f, zPos);
                uvs[v] = new Vector2((float)x / xSegments, (float)z / zSegments);
                v++;
            }
        }

        int t = 0;

        for (int z = 0; z < zSegments; z++)
        {
            for (int x = 0; x < xSegments; x++)
            {
                int i = z * (xSegments + 1) + x;

                triangles[t++] = i;
                triangles[t++] = i + xSegments + 1;
                triangles[t++] = i + 1;

                triangles[t++] = i + 1;
                triangles[t++] = i + xSegments + 1;
                triangles[t++] = i + xSegments + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}