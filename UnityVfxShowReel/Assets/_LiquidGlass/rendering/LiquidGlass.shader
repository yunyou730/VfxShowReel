Shader "Ayy/LiquidGlass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius",Range(0,1)) = 0.5
        _CenterX ("CenterX",Range(-1,1)) = 0
        _CenterY ("CenterY",Range(-1,1)) = 0
        
        _Offset("Offset",Range(0,1)) = 0.1
        _PowFactor("PowFactor",Range(1.0,10.0)) = 5.0

        _Color("Color",Color) = (1,1,1,1)
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

            float _Radius;
            float _CenterX,_CenterY;
            float _Offset;
            float _PowFactor;

            float4 _Color;
            
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

            half4 frag (v2f i) : SV_Target
            {
                float ratio = _MainTex_TexelSize.z / _MainTex_TexelSize.w;  // 宽度 : 高度

                float2 texelSize = _MainTex_TexelSize.xy;
                float2 originUV = i.uv;

                float2 uv = convertCoord(originUV,ratio);
                float2 center = convertCoord(float2(_CenterX,_CenterY),ratio);
                float2 dir = uv - center;

                half4 ret = half4(0.0,0.0,0.0,1.0);
                float dis = length(dir); 
                if (dis < _Radius)
                {
                    dir = normalize(dir) * _Offset * pow(dis,_PowFactor);
                    
                    //uv += dir;

                    uv = originUV - dir;
                    ret = tex2D(_MainTex,uv) * _Color;
                    //ret = float4(dir,0.0,1.0);
                    //return float4(1.0,1.0,0.0,1.0);
                }
                else
                {
                    ret = tex2D(_MainTex,originUV);    
                }
                return ret;
            }
            ENDHLSL
        }
    }
}
