Shader "UWA/Blur"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			Name"SceneBlur"
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			struct Attributes
			{
				float4 positionOS       : POSITION;
				float2 uv               : TEXCOORD0;
			};

			struct Varyings
			{
				float4 vertex : SV_POSITION;
				float2 uv	: TEXCOORD0;
				float4 uv01 : TEXCOORD1;
				float4 uv23 : TEXCOORD2;
				float4 uv45 : TEXCOORD3;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 offsets;
			float4 tintColor;

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			Varyings vert(Attributes input) {
				Varyings output = (Varyings)0;
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
				output.vertex = vertexInput.positionCS;
				output.uv = input.uv;

				output.uv01 = input.uv.xyxy + offsets.xyxy * float4(1, 1, -1, -1);
				output.uv23 = input.uv.xyxy + offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
				output.uv45 = input.uv.xyxy + offsets.xyxy * float4(1, 1, -1, -1) * 3.0;

				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				half4 color = float4 (0,0,0,0);

				color += 0.40 *  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
				color += 0.15 *  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv01.xy);
				color += 0.15 *  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv01.zw);
				color += 0.10 *  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv23.xy);
				color += 0.10 *  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv23.zw);
				color += 0.05 *  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv45.xy);
				color += 0.05 *  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv45.zw);

				return color;
			}

			#pragma vertex vert
			#pragma fragment frag

			ENDHLSL
		}
	}
		FallBack "Diffuse"
}
