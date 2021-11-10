using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using Random = System.Random;

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
	public float steepness = 2.0f;
	[Range(0, 10)]
	public float height = 2.0f;

	[Header("Caves")]
	[Range(1, 5)]
	public float cave_Smoothness = 1;
	[Range(1, 5)]
	public float terraces = 1.5f;
	[Range(0.5f, 10.0f)]
	public float scale;
	[Range(0.5f, 20.0f)]
	public float fillness;

	[Header("Depth")]
	[Range(0, 2.0f)]
	public float warpFrequency = 0.04f;
	[Range(0, 20)]
	public float warpAmplitude = 4.0f;
	[Range(1, 200)]
	public float depthFillness = 20.0f;
	[Range(80, 100)]
	public float inBetweenLevelMerge = 90.0f;

	[Header("Noise")]
	[HideInInspector]
	public int seed;
	[HideInInspector]
	public int numOctaves = 4;
	[HideInInspector]
	public float lacunarity = 2;
	[HideInInspector]
	public float persistence = 0.5f;
	[HideInInspector]
	public float noiseScale = 1;
	[HideInInspector]
	public float noiseWeight = 1.0f;
	[HideInInspector]
	public bool closeEdges;
	[HideInInspector]
	public float floorOffset = 1;
	[HideInInspector]
	public float weightMultiplier = 1;
	[HideInInspector]
	public float hardFloorHeight;
	[HideInInspector]
	public float hardFloorWeight;
	[HideInInspector]
	public Vector4 shaderParams;


	[Header("World")]
	public float isoLevel;
	public Vector3 boundsSize;
	public Vector3 offset = Vector3.zero;

	[Header("Vegetation")] 
	public List<GameObject> vegetation;
	[Range(0.0f, 6.0f)]
	public float heightStart, heightEnd;
	[Range(0, 1.0f)]
	public float vegetationProbability;
	
	public Transform vegetationParent;
	
	[HideInInspector]
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
		mesh = new Mesh();
		MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.receiveShadows = true;
        mat = renderer.material;
		CreateBuffers();
		GenerateNewTerrain();
    }

	public void GenerateNewTerrain(bool vegetation = false){
		GenerateDensity();
		Generate();
		
		if (vegetation){
			GenerateVegetation();
		}
	}
    

    void Update()
    {

		//Generate();

	}
    private void OnValidate()
    {
		/*GenerateDensity();
		Generate();*/
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

		// caves params
		densityShader.SetFloat("_fillness", fillness);
		densityShader.SetFloat("_caveSmoothness", cave_Smoothness);
		densityShader.SetFloat("_terraces", 5.1f - terraces);
		densityShader.SetFloat("_scale", scale);

		// Depth
		densityShader.SetFloat("_warpFrequency", warpFrequency);
		densityShader.SetFloat("_warpAmplitude", warpAmplitude);
		densityShader.SetFloat("_depthFillness", depthFillness);
		densityShader.SetFloat("_intermediate", (101.0f-inBetweenLevelMerge));

		

		densityShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
		Array.Clear(offsets, 0, offsets.Length);
		offsetsBuffer.Release();
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


		mesh.Clear();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = verticesArray;
        mesh.triangles = trianglesArray;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

       
        //numPolysBuffer.Release();
        verticesBuffer.Release();
        trianglesBuffer.Release();
		
		Array.Clear(polysArray, 0, numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis);
		Array.Clear(prefixSumArray, 0, numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis);
		Array.Clear(verticesArray, 0, 3 * totalPolygs);
		Array.Clear(trianglesArray, 0, 3 * totalPolygs);

	}

    private void GenerateVegetation(){
	    ClearVegetation();
	    
	    Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
	    Vector3[] vertices = mesh.vertices;
	    Matrix4x4 localToWorld = transform.localToWorldMatrix;
	    System.Random random = new System.Random ();
	    GameObject tree;
 
	    for (int i = 0; i < vertices.Length; i++) {
		    Vector3 world_v = localToWorld.MultiplyPoint3x4(vertices[i]);

		    if (random.NextDouble() < vegetationProbability && world_v.y > heightStart && world_v.y < heightEnd) {
			    //Vector3 spawnPoint = world_v;
			    tree = Instantiate (vegetation[UnityEngine.Random.Range(0, vegetation.Count)] , vertices[i], Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0));
			    tree.transform.SetParent(vegetationParent.transform);
		    }
	    }
    }

    private void ClearVegetation(){
	    GameObject[] oldVegetation = GameObject.FindGameObjectsWithTag("Vegetation");

	    foreach (GameObject model in oldVegetation){
		    Destroy(model);
	    }
    }
}
