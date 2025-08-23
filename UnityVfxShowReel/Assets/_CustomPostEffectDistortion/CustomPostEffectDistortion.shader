Shader "Ayy/CustomPostEffectDistortion"
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

			float _CenterX;
			float _CenterY;
			float _LowerThreshold;
			float _IncThreshold;
			float _DecThreshold;

			float _ZoomFactor;
			float _DebugDistortionStrength;

			float _Mode;
			float _WaveFreq;
    		float _WaveAmplitude;

			float _ZoomerInner;
			float _ZoomerOuter;
			float _ZoomerZoomFactor;

			float _TestScale;
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


			float GetDistortionStrength(float2 uv)
			{
				float2 center = float2(_CenterX,_CenterY);

				float ratio = _ScreenParams.x / _ScreenParams.y;
				uv = uv * 2.0 - 1.0;
				uv.x *= ratio;

				center = center * 2.0 - 1.0;
				center.x *= ratio;

				float v = length(uv - center);
				if (abs(_Mode - 2.0) < 0.1)		// sin wave 模式
				{
					return sin(v + _Time.y * _WaveFreq) * _WaveAmplitude * 0.5 + 0.5;
				}
				if (abs(_Mode - 1.0) < 0.1)		// zoomer 模式
				{
					return smoothstep(_ZoomerInner + _ZoomerOuter,_ZoomerInner,v) * _ZoomerZoomFactor;
				}

				// auto expanding 模式
				float v1 = smoothstep(_LowerThreshold + _IncThreshold,_LowerThreshold,v);
				return v1;
			}
			
			half4 Frag(Varyings input) : SV_Target
			{
				float2 uv = input.uv.xy;
				float distortStrength = GetDistortionStrength(uv);

				if (_DebugDistortionStrength > 0.5)
				{
					return float4(distortStrength,distortStrength,distortStrength,1.0);
				}

				float scale = 1.0 + distortStrength * _ZoomFactor;
				float2 sampleUV = uv;
				sampleUV = sampleUV - float2(_CenterX,_CenterY);
				sampleUV /= scale;
				sampleUV = sampleUV + float2(_CenterX,_CenterY);
				float4 col = tex2D(_MainTex,sampleUV);
				return col;
				//return lerp(col,float4(1,1,1,1),distortStrength);
			}			


			half4 Frag1(Varyings input) : SV_Target
			{
				float2 uv = input.uv.xy;
				return tex2D(_MainTex,uv);
			}

			half4 Frag2(Varyings input) : SV_Target
			{
				float2 uv = input.uv.xy;

				uv -= float2(0.5,0.5);
				uv /= 3.0f;
				uv += float2(0.5,0.5);
				
				return tex2D(_MainTex,uv);
			}

			half4 Frag3(Varyings input) : SV_Target
			{
				float2 uv = input.uv.xy;

				uv -= float2(_CenterX,_CenterY);
				uv /= 3.0f;
				uv += float2(_CenterX,_CenterY);
				
				return tex2D(_MainTex,uv);
			}

			half4 Frag4(Varyings input) : SV_Target
			{
				float2 uv = input.uv.xy;

				uv -= float2(_CenterX,_CenterY);
				uv /= _TestScale;
				uv += float2(_CenterX,_CenterY);
				
				return tex2D(_MainTex,uv);
			}

			half4 Frag5(Varyings input) : SV_Target
			{
				float2 uv = input.uv.xy;
				
				float2 center = float2(_CenterX,_CenterY);
				float ratio = _ScreenParams.x / _ScreenParams.y;
				float2 uv2 = uv * 2.0 - 1.0;
				uv2.x *= ratio;
				center = center * 2.0 - 1.0;
				center.x *= ratio;
				float v = length(uv2 - center);
				float distortionStrength = smoothstep(_ZoomerInner + _ZoomerOuter,_ZoomerInner,v) * _ZoomerZoomFactor;
				
				float scale = 1.0 + distortionStrength * _ZoomFactor;
				uv -= float2(_CenterX,_CenterY);
				uv /= scale;
				uv += float2(_CenterX,_CenterY);
				
				return tex2D(_MainTex,uv);
			}

			half4 Frag6(Varyings input) : SV_Target
			{
				float2 uv = input.uv.xy;
				
				float2 center = float2(_CenterX,_CenterY);
				float ratio = _ScreenParams.x / _ScreenParams.y;
				float2 uv2 = uv * 2.0 - 1.0;
				uv2.x *= ratio;
				center = center * 2.0 - 1.0;
				center.x *= ratio;
				float v = length(uv2 - center);
				float distortionStrength = sin(v + _Time.y * _WaveFreq) * _WaveAmplitude * 0.5 + 0.5;
				
				float scale = 1.0 + distortionStrength * _ZoomFactor;
				uv -= float2(_CenterX,_CenterY);
				uv /= scale;
				uv += float2(_CenterX,_CenterY);
				
				return tex2D(_MainTex,uv);
			}			

			ENDHLSL
		}

		Pass
		{
			Name "Chessboard"
			Tags{"LightMode" = "UniversalForwardOnly"}
			
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
			float _CellsNum;
			float _GridBorder;

			float4 _LineColor;
			float4 _BgColor1;
			float4 _BgColor2;
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
				float2 origUV = uv;

				// line
				float ratio = _ScreenParams.x / _ScreenParams.y;
				uv = uv * 2.0 - 1.0;
				uv.x *= ratio;
				uv = frac(uv * _CellsNum);
				float vy = smoothstep(0.0,_GridBorder,uv.y) * smoothstep(_GridBorder,0.0,uv.y) * 4.0;
				float vx = smoothstep(0.0,_GridBorder,uv.x) * smoothstep(_GridBorder,0.0,uv.x) * 4.0;
				float v = clamp(vx + vy,0.0,1.0);

				// bg
				float4 bgColor = lerp(_BgColor1,_BgColor2,smoothstep(0.0,2.0,origUV.x + origUV.y));
				return lerp(bgColor,_LineColor,v);
			}
			ENDHLSL			
		}
    }
}
