Shader "UWA/OutlineBasedGeometricSpace"
{
	Properties
	{
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline Width",Range(.002,0.03)) = 0.05
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		//表面剔除，剔除正面
		Cull Front
		//开启深度写入
		ZWrite On
		//通道遮罩
		ColorMask RGB
		//开启混合
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Name"OUTLINE"

			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			CBUFFER_START(UnityPerMaterial)
			float _OutlineWidth;
			float4 _OutlineColor;
			CBUFFER_END

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				//Instancing
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				half fogCoord : TEXCOORD0;
				half4 color : COLOR;
				//声明顶点是否在视域体内，用于筛选这个顶点是否输出到片段着色器
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);

				input.positionOS.xyz += input.normalOS.xyz * _OutlineWidth;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
				output.positionCS = vertexInput.positionCS;

				output.color = _OutlineColor;
				output.fogCoord = ComputeFogFactor(output.positionCS.z);
				return output;
			}

			half4 frag(Varyings i) : SV_Target
			{
				i.color.rgb = MixFog(i.color.rgb, i.fogCoord);
				return i.color;
			}
			ENDHLSL
		}
    }
}
