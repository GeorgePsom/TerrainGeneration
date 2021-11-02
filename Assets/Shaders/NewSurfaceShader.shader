Shader "Custom/NewSurfaceShader"
{
    Properties {

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxColourCount = 8;

		int baseColourCount;
		float3 baseColours[maxColourCount];
		float baseStartHeights[maxColourCount];

		float minHeight = 0;
		float maxHeight = 100;

		struct Input {
			float3 worldPos;
			//float3 normal;
		};

		float inverseLerp(float a, float b, float value) {
			return saturate((value-a)/(b-a));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			//float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
			
				//float drawStrength = saturate(sign(heightPercent - baseStartHeights[i]));
				//o.Albedo = o.Albedo * (1-drawStrength) + baseColours[i] * drawStrength;
				fixed4 col;
				if (IN.worldPos.y >= -5 && IN.worldPos.y < 0){
                    col = fixed4(0.663f, 0.663f, 0.663, 1.0f);
                }
                else if(IN.worldPos.y >= 0 && IN.worldPos.y < 2){
                    col = fixed4(1.0f, 0.8509f, 0.6666f, 1.0f);
                }
                else if(IN.worldPos.y >= 2 && IN.worldPos.y < 10){
                    col = fixed4(0.0f, 1.0f, 0.0f, 1.0f);
                }
                else if(IN.worldPos.y >= 10 && IN.worldPos.y < 17){
                    col = fixed4(0.5450f, 0.2705f, 0.0745f, 1.0f);
                }
                else{
                    col = fixed4(0.8274f, 0.8274f, 0.8274f, 1.0f);
                }
				o.Albedo = col;
			
		}


		ENDCG
	}
	FallBack "Diffuse"
}
