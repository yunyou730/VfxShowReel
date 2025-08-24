Shader "Ayy/OutlineFlameByNoise/FlameByMask"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
    }

    SubShader
    {
		Tags{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"RenderPipeline" = "UniversalPipeline"
		}
		Blend Off
		ZWrite Off
		ZTest Off
		Cull Back
		
		Pass
		{
			Name "CustomDistortion"
			Tags{"LightMode" = "UniversalForwardOnly"}

			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
			sampler2D _MainTex;
			float4 _MainTex_ST;
			CBUFFER_END

			struct Attributes
			{
			    float4 positionOS : POSITION;
			    float4 uv : TEXCOORD0;
			};

			struct Varyings
			{
			    float4 positionHCS : SV_POSITION;
			    float4 uv : TEXCOORD0;
			};

			Varyings Vert(Attributes IN)
			{
			    Varyings OUT;
				OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
			    OUT.uv = IN.uv;
			    return OUT;
			}
			
			half4 Frag(Varyings input) : SV_Target
			{
				float2 uv = input.uv.xy;
				float4 col = tex2D(_MainTex,uv);
				return col;
			}			
			

			ENDHLSL
		}
    }
}
