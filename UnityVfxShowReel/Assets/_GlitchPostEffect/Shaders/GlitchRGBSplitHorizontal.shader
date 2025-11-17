Shader "ayy/Glitch/GlitchRGBSplitHorizontal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity("Intensity",Range(0,1)) = 1
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

            float _Intensity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float rand2D(float2 p)
            {
                return frac(sin(dot(p,float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float split = _Intensity * rand2D(float2(_Time.y,2.0));
                float2 uv = i.uv;
                float r = tex2D(_MainTex,uv + float2(split,0.0)).r;
                float g = tex2D(_MainTex,uv + float2(0.0,0.0)).g;
                float b = tex2D(_MainTex,uv + float2(-split,0.0f)).b;
                return float4(r,g,b,1.0);
            }
            ENDCG
        }
    }
}
