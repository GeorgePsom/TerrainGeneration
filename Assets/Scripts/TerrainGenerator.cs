using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    public int xSize, ySize;
    public float Scale;


    private Mesh mesh;
    private Material mat;

    private Vector3[] vertices;
    private int[] triangles;

    private ComputeShader cs;

    private void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.receiveShadows = true;
        mat = renderer.material;
        ComputeShader cs = (ComputeShader)Resources.Load("HeightmapCompute");
        Generate();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnValidate()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.name = "Procedural Grid";
        
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                float xPos = (float)x * Scale;
                float yPos = (float)y * Scale;
                vertices[i] = new Vector3(xPos, 0.0f, yPos);
                uv[i] = new Vector2((float)xPos / (Scale * xSize), (float)yPos / (Scale * ySize));
                tangents[i] = tangent;

            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        // Triangle Topology
        int[] triangles = new int[6 * xSize * ySize];

        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {

                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
    }
}
