#ifndef MARCHING_CUBES_VAR_INCLUDED
#define MARCHING_CUBES_VAR_INCLUDED

//struct marchingTriangles
//{
//	int triangles[16];
//};

StructuredBuffer<int> _TriangleTable;
Texture3D<float4> _TerrainMap;
float3 _Dims;
SamplerState MyLinearRepeatSampler;

#endif