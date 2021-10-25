#ifndef MARCHING_CUBES_VAR_INCLUDED
#define MARCHING_CUBES_VAR_INCLUDED

struct marchingTriangles
{
	int triangles[16];
};

StructuredBuffer<marchingTriangles> _TriangleTable;
StructuredBuffer<float> _TerrainMap;

#endif