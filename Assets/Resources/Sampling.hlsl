#ifndef SAMPLING_INCLUDED
#define SAMPLING_INCLUDED

float4 TrilinearSampling(in Texture3D<float4> tex, int3 dims, float3 uvw)
{
	float4 result;
	uvw = frac(uvw);
	const float3 pixelCoord = uvw.xyz * dims.xyz;
	float3 pixelCoordCenters = pixelCoord - 0.5f.xxx;

	pixelCoordCenters = clamp(pixelCoordCenters, 0.0, float3(dims.x,dims.y, dims.z) - 1.0);

	const uint3 pixelCoordCentersBotLeft = floor(pixelCoordCenters);

	const float3 pixelCoordCentersFrac = frac(pixelCoordCenters);

	// Repeat mode

	const uint3 backBotLeft = pixelCoordCentersBotLeft;
	const uint3 maxCoord = backBotLeft + uint3(1, 1, 1);
	const uint3 doNotRepeat = uint3(
		maxCoord.x >= dims.x - 1 ? 0 : 1,
		maxCoord.y >= dims.y - 1 ? 0 : 1,
		maxCoord.z >= dims.z - 1 ? 0 : 1);

	// Back z
	const uint3 backBotRight = (backBotLeft + uint3(1, 0, 0)) * uint3(doNotRepeat.x, 1, 1);
	const uint3 backTopLeft = (backBotLeft + uint3(0, 1, 0)) * uint3(1, doNotRepeat.y, 1);
	const uint3 backTopRight = (backBotLeft + uint3(1, 1, 0)) * uint3(doNotRepeat.x, doNotRepeat.y, 1);


	const float4 backDataBotLeft = tex[backBotLeft];
	const float4 backDataBotRight = tex[backBotRight];
	const float4 backDataTopLeft = tex[backTopLeft];
	const float4 backDataTopRight = tex[backTopRight];

	const float4 back = lerp(
		lerp(backDataBotLeft, backDataBotRight, pixelCoordCentersFrac.x),
		lerp(backDataTopLeft, backDataTopRight, pixelCoordCentersFrac.x),
		pixelCoordCentersFrac.y
	);

	// Forward z
	const uint3 forwardBotLeft = (backBotLeft + uint3(0, 0, 1)) * uint3(1, 1, doNotRepeat.z);
	const uint3 forwardBotRight = (forwardBotLeft + uint3(1, 0, 1)) * uint3(doNotRepeat.x, 1, doNotRepeat.z);
	const uint3 forwardTopLeft = (forwardBotLeft + uint3(0, 1, 1)) * uint3(1, doNotRepeat.y, doNotRepeat.z);
	const uint3 forwardTopRight = (forwardBotLeft + uint3(1, 1, 1)) * uint3(doNotRepeat.x, doNotRepeat.y, doNotRepeat.z);


	const float4 forwardDataBotLeft = tex[forwardBotLeft];
	const float4 forwardDataBotRight = tex[forwardBotRight];
	const float4 forwardDataTopLeft = tex[forwardTopLeft];
	const float4 forwardDataTopRight = tex[forwardTopRight];

	const float4 forward = lerp(
		lerp(forwardDataBotLeft, forwardDataBotRight, pixelCoordCentersFrac.x),
		lerp(forwardDataTopLeft, forwardDataTopRight, pixelCoordCentersFrac.x),
		pixelCoordCentersFrac.y
	);

	result = lerp(
		back,
		forward,
		pixelCoordCentersFrac.z
	);
	
	return result;
}



#endif