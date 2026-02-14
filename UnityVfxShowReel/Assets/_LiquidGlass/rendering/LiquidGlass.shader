Shader "Ayy/LiquidGlass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius",Range(0,1)) = 0.5
        _CenterX ("CenterX",Range(-1,1)) = 0
        _CenterY ("CenterY",Range(-1,1)) = 0
        
        _Offset("Offset",Range(0,0.3)) = 0.15
        _PowFactor("PowFactor",Range(1.0,5.0)) = 2.5

        _Color("Color",Color) = (1,1,1,1)
        _BlurEdge("Blur Edge",Range(0,0.3)) = 0.05
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

            half4 frag (v2f i) : SV_Target
            {
                float ratio = _MainTex_TexelSize.z / _MainTex_TexelSize.w;  // 宽度 : 高度

                
                float2 originUV = i.uv;

                float2 uv = convertCoord(originUV,ratio);
                float2 c1 = convertCoord(float2(_CenterX,_CenterY),ratio);

                float sd1 = sdfCircle(uv,c1,_Radius);
                float sdf = sd1;

                float2 dir = uv - c1;
                float dis = length(dir);

                float smoothEdge = step(sdf,_BlurEdge);
                float distortion = lerp(0.0,_Offset * pow(dis,_PowFactor),smoothEdge);
                
                //return float4(distortion,distortion,distortion,1.0);

                float2 uvOffset = normalize(dir) * -1.0 * distortion;
                uv = originUV + uvOffset;

                
                // 原图,模糊图, 根据 sdf值做混合 
                half4 originTexColor = tex2D(_MainTex,uv);
                half4 blurTexColor = tex2D(_LiquidBlurRenderTexture,uv);
                blurTexColor *= _Color;
                half4 ret = lerp(originTexColor,blurTexColor,smoothEdge);
                return ret;
            }
            ENDHLSL
        }
    }
}
