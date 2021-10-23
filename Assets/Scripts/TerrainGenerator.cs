using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    public int xSize, ySize;
    public float Scale;

    public bool generate3DNoise = false;
    private Mesh mesh;
    private Material mat;
    public Slicer slicer;
    public perlinNoiseGenerator noiseGenerator;
    public int Octaves;
    private List<Vector3> vertices;
    private List<int> triangles;
    private RenderTexture rt;
    public ComputeShader cs;
    public ComputeShader perlinNoiseCS;
    public Texture3D perlinNoise3D;
    private RenderTexture perlinNoise3DRT;
    

    private void Awake()
    {
        

    }
    
    

    // Start is called before the first frame update
    void Start()
    {
       
        GeneratePerlinNoise3D();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.receiveShadows = true;
        mat = renderer.material;
       
        rt = new RenderTexture(64, 64, 0);
        rt.enableRandomWrite = true;
        rt.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        rt.format = RenderTextureFormat.ARGB32;
        rt.volumeDepth = 64;
        rt.Create();
        Generate();
       



    }

    

    
    void Update()
    {
        //GeneratePerlinNoise3D();
        //Generate();
    }
    private void OnValidate()
    {
        //Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.name = "Procedural Grid";
        triangles = new List<int>();
        vertices = new List<Vector3>();
        cs.SetTexture(0, "_Density", rt);
        
        cs.SetInt("_Octaves", Octaves);
        cs.SetTexture(0, "_PerlinNoise", perlinNoise3DRT);
        cs.Dispatch(0, 8, 8, 8);
        slicer.Save(rt, "Density");
        

        //Vector2[] uv = new Vector2[vertices.Length];
        //Vector4[] tangents = new Vector4[vertices.Length];
        //Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        //for (int i = 0, y = 0; y <= ySize; y++)
        //{
        //    for (int x = 0; x <= xSize; x++, i++)
        //    {
        //        float xPos = (float)x * Scale;
        //        float yPos = (float)y * Scale;
        //        vertices[i] = new Vector3(xPos, 0.0f, yPos);
        //        uv[i] = new Vector2((float)xPos / (Scale * xSize), (float)yPos / (Scale * ySize));
        //        tangents[i] = tangent;

        //    }
        //}
        //mesh.vertices = vertices;
        //mesh.uv = uv;
        //mesh.tangents = tangents;

        //// Triangle Topology
        //int[] triangles = new int[6 * xSize * ySize];

        //for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        //{
        //    for (int x = 0; x < xSize; x++, ti += 6, vi++)
        //    {

        //        triangles[ti] = vi;
        //        triangles[ti + 3] = triangles[ti + 2] = vi + 1;
        //        triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
        //        triangles[ti + 5] = vi + xSize + 2;
        //    }
        //}
        //mesh.triangles = triangles;
    }

    private void GeneratePerlinNoise3D()
    {

        perlinNoise3DRT = new RenderTexture(64, 64, 0);

        perlinNoise3DRT.enableRandomWrite = true;
        perlinNoise3DRT.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        perlinNoise3DRT.format = RenderTextureFormat.ARGB32;
        perlinNoise3DRT.volumeDepth = 64;
        perlinNoise3DRT.Create();
        
        ComputeBuffer perlinBuffer = new ComputeBuffer(64 * 64 * 64, sizeof(float));
        float[] perlinArray = new float[64 * 64 * 64];

        //// Configure the texture
        int size = 64;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[size * size * size];

        // Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
        float inverseResolution = 1.0f / (size - 1.0f);
        for (int z = 0; z < size; z++)
        {
            int zOffset = z * size * size;
            for (int y = 0; y < size; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    float color = perlinNoiseGenerator.get3DPerlinNoise(new Vector3(x, y, z), 1.01f);
                    perlinArray[x + yOffset + zOffset] = color;
                    colors[x + yOffset + zOffset] = new Color(color, color, color);
                }
            }
        }

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();

        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(texture, "Assets/PerlinNoise3DTexture.asset");
        perlinBuffer.SetData(perlinArray);
        perlinNoiseCS.SetTexture(0, "_PerlinNoise3D", perlinNoise3DRT);
        perlinNoiseCS.SetBuffer(0, "_PerlinNoiseBuffer", perlinBuffer);
        perlinNoiseCS.Dispatch(0, 64 / 8, 64 / 8, 64 / 8);
    }
}
