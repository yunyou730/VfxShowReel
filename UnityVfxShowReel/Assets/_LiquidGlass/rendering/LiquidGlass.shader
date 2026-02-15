Shader "Ayy/LiquidGlass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CenterX ("CenterX",Range(-1,1)) = 0
        _CenterY ("CenterY",Range(-1,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            
            #pragma vertex vert
            #pragma fragment frag
            
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
            float4 _MainTex_TexelSize;  // x: 1/w, y: 1/h, z:w, w:h

            // 经过模糊后的图像
            sampler2D _LiquidBlurRenderTexture; 
            

            float _Radius;
            float _CenterX,_CenterY;
            float _Offset;
            float _PowFactor;

            float4 _Color;
            float _BlurEdge;
            float _AAEdge;

            #pragma shader_feature _ENABLE_DEBUG_BLUR_WEIGHT         // 调试 模糊权重
            #pragma shader_feature _ENABLE_DEBUG_DISTORTION_WEIGHT   // 调试 扭曲权重

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 convertCoord(float2 uv,float ratio)
            {
                uv = uv * 2.0 - 1.0;    // [0,1] -> [-1,1]
                uv.x *= ratio;  // ratio
                return uv;
            }

            float sdfCircle(float2 uv,float2 center,float radius)
            {
                return distance(uv,center) - radius;
            }
            
            float sdfSquare(float2 uv, float2 center, float sideLength)
            {
                float2 localUV = uv - center;
                float halfSide = sideLength * 0.5;
                float2 distToEdge = abs(localUV) - halfSide;
                float outsideDist = length(max(distToEdge, 0.0));
                float insideDist = min(max(distToEdge.x, distToEdge.y), 0.0);
                return outsideDist + insideDist;
            }

            half4 frag (v2f i) : SV_Target
            {
                float ratio = _MainTex_TexelSize.z / _MainTex_TexelSize.w; // 宽高比
                float2 originUV = i.uv;

                // 构建 SDF
                float2 uv = convertCoord(originUV,ratio);
                float2 c1 = convertCoord(float2(_CenterX,_CenterY),ratio);
                float sd1 = sdfSquare(uv,c1,_Radius);
                //float sd1 = sdfCircle(uv,c1,_Radius);
                float sdf = sd1;

                // 模糊 和 清晰图 的权重.
                float smoothEdge = smoothstep(_BlurEdge + _AAEdge, _BlurEdge - _AAEdge, sdf);
                #if _ENABLE_DEBUG_BLUR_WEIGHT    // debug, 显示权重
                return float4(smoothEdge,smoothEdge,smoothEdge,1.0);    
                #endif 

                // uv distortion 强度的权重
                float2 dir = uv - c1;
                float dis = length(dir);
                float distortion = lerp(0.0,_Offset * pow(dis,_PowFactor),smoothEdge);

                #if _ENABLE_DEBUG_DISTORTION_WEIGHT    // debug, 显示权重
                return float4(distortion,distortion,distortion,1.0);
                #endif

                // 扭曲 uv distortion
                float2 uvOffset = normalize(dir) * -1.0 * distortion;        
                uv = originUV + uvOffset;

                // 原图,模糊图, 根据 sdf值做混合 
                half4 originTexColor = tex2D(_MainTex,uv);
                half4 blurTexColor = tex2D(_LiquidBlurRenderTexture,uv) * _Color;
                half4 ret = lerp(originTexColor,blurTexColor,smoothEdge);
                return ret;
            }
            ENDHLSL
        }
    }
}
