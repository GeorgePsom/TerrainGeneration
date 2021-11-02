using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    
    public ComputeShader densityShader;
    public ComputeShader prefixSumCS;
    public ComputeShader numPolygonsCS;
	public ComputeShader marchingCubesCS;

	[Header("Mountaints")]
	[Tooltip("Value should be between 5-64.")]
	[Range(1, 128)]
	public float mountainous = 32.0f;
	[Range(1, 16)]
	public float smoothness = 5;
	[Range(0, 10)]
	public float steepness;
	[Range(0, 10)]
	public float height;

	[Header("Noise")]
	public int seed;
	public int numOctaves = 4;
	public float lacunarity = 2;
	public float persistence = 0.5f;
	public float noiseScale = 1;
	public float noiseWeight = 1.0f;
	public bool closeEdges;
	public float floorOffset = 1;
	public float weightMultiplier = 1;
	public float hardFloorHeight;
	public float hardFloorWeight;
	public Vector4 shaderParams;

	[Header("World")]
	public float isoLevel;
	public Vector3 boundsSize;
	public Vector3 offset = Vector3.zero;


	
	[Range(2, 2048)]
	public int numPointsPerAxis = 30;

	private MeshFilter meshFilter;
	private Mesh mesh;
	private Material mat;

	// Buffers
	private ComputeBuffer pointsBuffer;
	private ComputeBuffer numPolysBuffer;
	private ComputeBuffer offsetsBuffer;



	// Start is called before the first frame update
	void Start()
    {
		meshFilter = GetComponent<MeshFilter>();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.receiveShadows = true;
        mat = renderer.material;
		CreateBuffers();
		GenerateDensity();
		Generate();
    }


    

    void Update()
    {

		//Generate();

	}
    private void OnValidate()
    {
		GenerateDensity();
		Generate();
    }

    private void CreateBuffers()
    {
		int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;

		int numVoxelsPerAxis = numPointsPerAxis - 1;
		int numThreadsPerAxis = Mathf.CeilToInt(numPointsPerAxis / (float)8);
		pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4);
		
		numPolysBuffer = new ComputeBuffer(numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis, sizeof(int));
		
	}
	private void GenerateDensity()
    {
		
		int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;

		int numVoxelsPerAxis = numPointsPerAxis - 1;
		int numThreadsPerAxis = Mathf.CeilToInt(numPointsPerAxis / (float)8);
		Vector3 pointSpacing = boundsSize / (numPointsPerAxis - 1);
		
		

		// Noise parameters
		var prng = new System.Random(seed);
		float offsetRange = 1000;
		var offsets = new Vector3[numOctaves];
		offsetsBuffer = new ComputeBuffer(offsets.Length, sizeof(float) * 3);
		for (int i = 0; i < numOctaves; i++)
		{
			offsets[i] = new Vector3((float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1) * offsetRange;
		}
		
		offsetsBuffer.SetData(offsets);
		densityShader.SetBuffer(0, "_points", pointsBuffer);
		densityShader.SetFloat("_numPointsPerAxis", numPointsPerAxis);
		densityShader.SetFloat("_isoLevel", isoLevel);
		Vector3 center = Vector3.zero;
		Vector3 worldBounds = boundsSize;
		densityShader.SetVector("_centre", new Vector4(center.x, center.y, center.z));
		densityShader.SetVector("_boundsSize", boundsSize);
		densityShader.SetVector("_offset", new Vector4(offset.x, offset.y, offset.z));
		densityShader.SetVector("_spacing", pointSpacing);
		densityShader.SetVector("worldSize", worldBounds);
		densityShader.SetFloat("_octaves", Mathf.Max(1, numOctaves));
		densityShader.SetFloat("_lacunarity", lacunarity);
		densityShader.SetFloat("_persistence", persistence);
		densityShader.SetFloat("_noiseScale", noiseScale);
		densityShader.SetFloat("_noiseWeight", noiseWeight);
		densityShader.SetBool("_closeEdges", closeEdges);
		densityShader.SetBuffer(0, "_offsets", offsetsBuffer);
		densityShader.SetFloat("_floorOffset", floorOffset);
		densityShader.SetFloat("_weightMultiplier", weightMultiplier);
		densityShader.SetFloat("_hardFloor", hardFloorHeight);
		densityShader.SetFloat("_hardFloorWeight", hardFloorWeight);
		densityShader.SetVector("_params", shaderParams);
		
		// mountain params
		densityShader.SetFloat("_mountainous", mountainous);
		densityShader.SetFloat("_smoothness", smoothness);
		densityShader.SetFloat("_mountainHeight", steepness);
		densityShader.SetFloat("_merging", height);

		densityShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

	}
	
    private void Generate()
    {
		int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;

		int numVoxelsPerAxis = numPointsPerAxis - 1;
		int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
		int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)8);
		
		

		numPolygonsCS.SetFloat("_isoLevel", isoLevel);
        numPolygonsCS.SetFloat("_numPointsPerAxis", numPointsPerAxis);
        numPolygonsCS.SetBuffer(0, "points", pointsBuffer);     
        numPolygonsCS.SetBuffer(0, "_NumOfPolygons", numPolysBuffer);
        numPolygonsCS.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

		int[] polysArray = new int[numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis];
        numPolysBuffer.GetData(polysArray);
       
        int iter = (int)System.Math.Ceiling(System.Math.Log(numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis, 2)) - 1;

        for (int i = 0; i < iter + 1; i++)
        {
            prefixSumCS.SetBuffer(0, "_pSum", numPolysBuffer);
            
            prefixSumCS.SetFloat("_numPointsPerAxis", numPointsPerAxis);
            prefixSumCS.SetFloat("_iter", i);
            prefixSumCS.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        }
        numPolysBuffer.GetData(polysArray);
        int[] prefixSumArray = new int[numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis];
        numPolysBuffer.GetData(prefixSumArray);
        int totalPolygs = prefixSumArray[prefixSumArray.Length - 1];

		// March cubes	
		ComputeBuffer verticesBuffer = new ComputeBuffer(3 * totalPolygs, 3 * sizeof(float));
		Vector3[] verticesArray = new Vector3[3 * totalPolygs];
        ComputeBuffer trianglesBuffer = new ComputeBuffer(3 * totalPolygs, sizeof(int));
        int[] trianglesArray = new int[3 * totalPolygs];

        marchingCubesCS.SetBuffer(0, "_Vertices", verticesBuffer);
        marchingCubesCS.SetBuffer(0, "_Triangles", trianglesBuffer);
		marchingCubesCS.SetFloat("isoLevel", isoLevel);
		marchingCubesCS.SetBuffer(0, "points", pointsBuffer);
		marchingCubesCS.SetFloat("numPointsPerAxis", numPointsPerAxis);
		marchingCubesCS.SetBuffer(0, "_PrefixSumPolygons", numPolysBuffer);
        marchingCubesCS.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);


        verticesBuffer.GetData(verticesArray);
        trianglesBuffer.GetData(trianglesArray);

		
		mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = verticesArray;
        mesh.triangles = trianglesArray;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

       
        //numPolysBuffer.Release();
        verticesBuffer.Release();
        trianglesBuffer.Release();
		offsetsBuffer.Release();

	}


}
