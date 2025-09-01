Shader "Ayy/OutlineFlameByNoise/FlameByMask"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
    	_DistortionStrength("DistortionStrength",Range(0,0.5)) = 0.1
    	_NoiseScale("NoiseScale",Range(-100,100)) = 20
    	_UVOffsetSpeedFactorY("UV Offset Speed Factor y",Range(-3,3)) = 1.0
    	_UVOffsetSpeedFactorX("UV Offset Speed Factor X",Range(-3,3)) = 1.0
    	
    	_ColorFrom("ColorFrom",Color) = (1,0,0,1)
    	_ColorTo("ColorFrom",Color) = (1,1,0,1)
    }

    SubShader
    {
		Tags{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"RenderPipeline" = "UniversalPipeline"
		}
		Blend SrcAlpha OneMinusSrcAlpha
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
			float _DistortionStrength;
			float _NoiseScale;
			float _UVOffsetSpeedFactorY;
			float _UVOffsetSpeedFactorX;
			float4 _ColorFrom;
    		float4  _ColorTo;
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

			// simple noise begin
			float noise_randomValue(float2 uv)
			{
				return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
			}

			float noise_interpolate(float a, float b, float t)
			{
			    return (1.0 - t) * a + (t * b);
			}
			
			float unity_valueNoise (float2 uv)
			{
			    float2 i = floor(uv);
			    float2 f = frac(uv);
			    f = f * f * (3.0 - 2.0 * f);

			    uv = abs(frac(uv) - 0.5);
			    float2 c0 = i + float2(0.0, 0.0);
			    float2 c1 = i + float2(1.0, 0.0);
			    float2 c2 = i + float2(0.0, 1.0);
			    float2 c3 = i + float2(1.0, 1.0);
			    float r0 = noise_randomValue(c0);
			    float r1 = noise_randomValue(c1);
			    float r2 = noise_randomValue(c2);
			    float r3 = noise_randomValue(c3);

			    float bottomOfGrid = noise_interpolate(r0, r1, f.x);
			    float topOfGrid = noise_interpolate(r2, r3, f.x);
			    float t = noise_interpolate(bottomOfGrid, topOfGrid, f.y);
			    return t;
			}

			float simpleNoise(float2 UV, float Scale)
			{
			    float t = 0.0;

			    float freq = pow(2.0, float(0));
			    float amp = pow(0.5, float(3-0));
			    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

			    freq = pow(2.0, float(1));
			    amp = pow(0.5, float(3-1));
			    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

			    freq = pow(2.0, float(2));
			    amp = pow(0.5, float(3-2));
			    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

				return t;
			}
			// simple noise end
			
			half4 Frag(Varyings input) : SV_Target
			{
				float2 uv = input.uv.xy;

				float2 noiseUV = uv;
				noiseUV.y += _UVOffsetSpeedFactorY * _Time.y;
				noiseUV.x += _UVOffsetSpeedFactorX * _Time.x;
				
				float offsetValue = simpleNoise(noiseUV,_NoiseScale);		// offsetValue [0,1]
				offsetValue = offsetValue * 2.0 - 1.0;		// offsetValue [0,1] => [-1,1]
				uv += offsetValue * _DistortionStrength;
				
				float4 col = tex2D(_MainTex,uv);
				col.rgb *= lerp(_ColorFrom,_ColorTo,smoothstep(-1,1,offsetValue)).rgb;
				return col;
			}
			ENDHLSL
		}
    }
}
