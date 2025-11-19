Shader "ayy/Glitch/LineBlockGlitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Strength ("Strength",Range(0,10)) = 1
        
        _LinesWidth ("LinesWidth",Float) = 0.1
        _Offset ("Offset",Range(0,1)) = 0.0
        _Threshold ("Threshold",Range(0,1)) = 0.5
    	
    	_Alpha ("Alpha",Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Strength;

            float _LinesWidth;
            float _Offset;
            float _Threshold;
            float _Alpha;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

	        float rand1D(float seed)
	        {
		        return frac(sin(seed * 12.9898) * 43758.5453123);
	        }

            float rand2D(float2 p)
            {
                return frac(sin(dot(p,float2(12.9898,78.233))) * 43758.5453);
            }

            float trunc(float x,float numLevels)
            {
                return floor(x * numLevels) / numLevels;
            }

			float3 rgb2yuv(float3 rgb)
			{
				float3 yuv;
				yuv.x = dot(rgb, float3(0.299, 0.587, 0.114));
				yuv.y = dot(rgb, float3(-0.14713, -0.28886, 0.436));
				yuv.z = dot(rgb, float3(0.615, -0.51499, -0.10001));
				return yuv;
			}
			
			float3 yuv2rgb(float3 yuv)
			{
				float3 rgb;
				rgb.r = yuv.x + yuv.z * 1.13983;
				rgb.g = yuv.x + dot(float2(-0.39465, -0.58060), yuv.yz);
				rgb.b = yuv.x + yuv.y * 2.03211;
				return rgb;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                //	[1] 生成随机强度梯度线条
            	float uv_trunc = rand1D(trunc(uv.y, 8) + floor(_Time.y * 4.0));
            	float uv_randomTrunc = 6.0 * floor(_Time.y * 24.0 * uv_trunc);
		        
		        // [2] 生成随机非均匀宽度线条
				float blockLine_random = 0.5 * rand1D(trunc(uv.y + uv_randomTrunc, 8 * _LinesWidth));
		        blockLine_random += 0.5 * rand1D(trunc(uv.y + uv_randomTrunc, 7));
		        blockLine_random = blockLine_random * 2.0 - 1.0;
            	float strength = (abs(blockLine_random) - _Threshold) * _Strength;
            	blockLine_random = sign(blockLine_random) * clamp(strength,0.0,1.0);
		        blockLine_random = lerp(0, blockLine_random, _Offset);
            	
                // [3] 生成源色调的blockLine Glitch
            	// 在原有颜色基础上, 只用 噪声灰度值,做了少许 uv 偏移 
		        float2 uv_blockLine = abs(saturate(uv + float2(0.1 * blockLine_random, 0)));
		        float4 blockLineColor = tex2D(_MainTex, uv_blockLine);

				// [4] 将RGB转到YUV空间，并做色调偏移
				// RGB -> YUV
				float3 blockLineColor_yuv = rgb2yuv(blockLineColor.rgb);
				// adjust Chrominance | 色度
				blockLineColor_yuv.y /= 1.0 - 3.0 * abs(blockLine_random) * saturate(0.5 - blockLine_random);
				// adjust Chroma | 浓度
				blockLineColor_yuv.z += 0.125 * blockLine_random * saturate(blockLine_random - 0.5);
				float3 blockLineColor_rgb = yuv2rgb(blockLineColor_yuv);

				// [5] 与源场景图进行混合
				float4 sceneColor = tex2D(_MainTex,uv);
				return lerp(sceneColor, float4(blockLineColor_rgb, blockLineColor.a), _Alpha);
            }
            ENDCG
        }
    }
}
