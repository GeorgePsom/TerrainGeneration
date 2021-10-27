#ifndef MARCHING_CUBES_INCLUDED
#define MARCHING_CUBES_INCLUDED


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


int GetCaseIndex(float4 corner0123, float4 corner4567, float terrainSurface)
{
	int caseNum = 0;
	
	caseNum |= (corner0123.x > terrainSurface) ? 1 << 0 : 0 << 0;
	caseNum |= (corner0123.y > terrainSurface) ? 1 << 1 : 0 << 1;
	caseNum |= (corner0123.z > terrainSurface) ? 1 << 2 : 0 << 2;
	caseNum |= (corner0123.w > terrainSurface) ? 1 << 3 : 0 << 3;

	caseNum |= (corner4567.x > terrainSurface) ? 1 << 4 : 0 << 4;
	caseNum |= (corner4567.y > terrainSurface) ? 1 << 5 : 0 << 5;
	caseNum |= (corner4567.z > terrainSurface) ? 1 << 6 : 0 << 6;
	caseNum |= (corner4567.w > terrainSurface) ? 1 << 7 : 0 << 7;

	return caseNum;
}
void getVoxelCorners(int3 voxel, out float4 corner0123, out float4 corner4567)
{
	float cube[8];
	
	for (int i = 0; i < 8; i++)
	{
		int3 corner = voxel + CornerTable[i];
		cube[i] = _TerrainMap[corner.x + corner.y * 65 + corner.z * 65 * 65];
	}
	corner0123 = float4(cube[0], cube[1], cube[2], cube[3]);
	corner4567 = float4(cube[4], cube[5], cube[6], cube[7]);
}



#endif