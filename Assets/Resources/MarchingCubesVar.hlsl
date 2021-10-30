#ifndef MARCHING_CUBES_VAR_INCLUDED
#define MARCHING_CUBES_VAR_INCLUDED


StructuredBuffer<int> _TriangleTable;
float _isoLevel;
StructuredBuffer<float4> _points;
float _numPointsPerAxis;
//Texture3D<float4> _TerrainMap;
//float3 _Dims;
//SamplerState MyLinearRepeatSampler;

#endif