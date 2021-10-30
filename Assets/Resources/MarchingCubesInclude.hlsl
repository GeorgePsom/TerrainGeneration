#ifndef MARCHING_CUBES_INCLUDED
#define MARCHING_CUBES_INCLUDED

//#include "Sampling.hlsl"
static int3 CornerTable[8] = 
{

		int3(0, 0, 0),
		int3(1, 0, 0),
		int3(1, 1, 0),
		int3(0, 1, 0),
		int3(0, 0, 1),
		int3(1, 0, 1),
		int3(1, 1, 1),
		int3(0, 1, 1)

};

static const int cornerIndexAFromEdge[12] = {
	0,
	1,
	2,
	3,
	4,
	5,
	6,
	7,
	0,
	1,
	2,
	3
};

static const int cornerIndexBFromEdge[12] = {
	1,
	2,
	3,
	0,
	5,
	6,
	7,
	4,
	4,
	5,
	6,
	7
};

static float3 EdgeTable[12][2] = 
{

	{ float3(0.0f, 0.0f, 0.0f), float3(1.0f, 0.0f, 0.0f) },
	{ float3(1.0f, 0.0f, 0.0f), float3(1.0f, 1.0f, 0.0f) },
	{ float3(0.0f, 1.0f, 0.0f), float3(1.0f, 1.0f, 0.0f) },
	{ float3(0.0f, 0.0f, 0.0f), float3(0.0f, 1.0f, 0.0f) },
	{ float3(0.0f, 0.0f, 1.0f), float3(1.0f, 0.0f, 1.0f) },
	{ float3(1.0f, 0.0f, 1.0f), float3(1.0f, 1.0f, 1.0f) },
	{ float3(0.0f, 1.0f, 1.0f), float3(1.0f, 1.0f, 1.0f) },
	{ float3(0.0f, 0.0f, 1.0f), float3(0.0f, 1.0f, 1.0f) },
	{ float3(0.0f, 0.0f, 0.0f), float3(0.0f, 0.0f, 1.0f) },
	{ float3(1.0f, 0.0f, 0.0f), float3(1.0f, 0.0f, 1.0f) },
	{ float3(1.0f, 1.0f, 0.0f), float3(1.0f, 1.0f, 1.0f) },
	{ float3(0.0f, 1.0f, 0.0f), float3(0.0f, 1.0f, 1.0f) }

};

int indexFromCoord(int x, int y, int z) {
	return z * (int)_numPointsPerAxis * (int)_numPointsPerAxis + y * (int)_numPointsPerAxis + x;
}
int GetCaseIndex(float4 corner0,  float4 corner1,
	 float4 corner2,  float4 corner3,
	 float4 corner4,  float4 corner5,
	 float4 corner6,  float4 corner7)
{
	int cubeIndex = 0;
	if (corner0.w < _isoLevel) cubeIndex |= 1;
	if (corner1.w < _isoLevel) cubeIndex |= 2;
	if (corner2.w < _isoLevel) cubeIndex |= 4;
	if (corner3.w < _isoLevel) cubeIndex |= 8;
	if (corner4.w < _isoLevel) cubeIndex |= 16;
	if (corner5.w < _isoLevel) cubeIndex |= 32;
	if (corner6.w < _isoLevel) cubeIndex |= 64;
	if (corner7.w < _isoLevel) cubeIndex |= 128;
	return cubeIndex;
}
void getVoxelCorners(int3 id,
	out float4 corner0, out float4 corner1,
	out float4 corner2, out float4 corner3,
	out float4 corner4, out float4 corner5,
	out float4 corner6, out float4 corner7)
{
	corner0 = _points[indexFromCoord(id.x, id.y, id.z)];
	corner1 = _points[indexFromCoord(id.x + 1, id.y, id.z)];
	corner2 = _points[indexFromCoord(id.x + 1, id.y, id.z + 1)];
	corner3 = _points[indexFromCoord(id.x, id.y, id.z + 1)];
	corner4 = _points[indexFromCoord(id.x, id.y + 1, id.z)];
	corner5 = _points[indexFromCoord(id.x + 1, id.y + 1, id.z)];
	corner6 = _points[indexFromCoord(id.x + 1, id.y + 1, id.z + 1)];
	corner7 = _points[indexFromCoord(id.x, id.y + 1, id.z + 1)];

}
	/*float cube[8];
	
	for (int i = 0; i < 8; i++)
	{
		int3 corner = voxel + CornerTable[i];
		float3 uvw = float3( (float)corner.x  / (_Dims.x + 1.0f), (float)corner.y  / (_Dims.y + 1.0f), (float)corner.z / (_Dims.z +  1.0f));
		cube[i] = _TerrainMap.SampleLevel(MyLinearRepeatSampler,  uvw, 64 );
	}
	corner0123 = float4(cube[0], cube[1], cube[2], cube[3]);
	corner4567 = float4(cube[4], cube[5], cube[6], cube[7]);
}*/



#endif