Shader "Ayy/BlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlitTemp ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                #if UNITY_UV_STARTS_AT_TOP
                    uv.y = 1.0 -uv.y;
                #endif

                if (_ProjectionParams.x < 0.0)
                {
                    uv.y = 1.0 - uv.y;
                }                
                
                half4 col = tex2D(_MainTex, uv);
                //half gray = dot(col.rgb, half3(0.299, 0.587, 0.114));
                //half gray = dot(col.rgb,half3(1.0,0.0,0.0));
                //col.rgb = half3(gray, gray, gray);

                col.rgb *= half3(1.0,0.0,0.0);
                return col;
            }
            ENDHLSL
        }
//        Pass
//        {
//            HLSLPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//
//            struct appdata
//            {
//                float4 vertex : POSITION;
//                float2 uv : TEXCOORD0;
//            };
//
//            struct v2f
//            {
//                float2 uv : TEXCOORD0;
//                float4 vertex : SV_POSITION;
//            };
//
//            sampler2D _MainTex;
//            float4 _MainTex_ST;
//
//            sampler2D _BlitTemp;
//
//            v2f vert (appdata v)
//            {
//                v2f o;
//                o.vertex = TransformObjectToHClip(v.vertex.xyz);
//                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                return o;
//            }
//
//            half4 frag (v2f i) : SV_Target
//            {
//                half4 col = tex2D(_BlitTemp, i.uv);
//                //half gray = dot(col.rgb, half3(0.299, 0.587, 0.114));
//                //half gray = dot(col.rgb,half3(1.0,0.0,0.0));
//                //col.rgb = half3(gray, gray, gray);
//                //col.rgb *= half3(1.0,0.0,0.0);
//                return col;
//            }
//            ENDHLSL
//        }
    }
    FallBack "Diffuse"
}
    