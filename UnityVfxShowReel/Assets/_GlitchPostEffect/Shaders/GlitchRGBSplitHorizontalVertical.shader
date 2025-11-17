Shader "ayy/Glitch/GlitchRGBSplitHorizontalVertical"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude("Amplitude",Range(0.1,5.0)) = 1.0
        _Amount("Amount",Range(0.0,5.0)) = 1.0
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

            float _Amplitude;
            float _Amount;
            
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
                float splitAmount = (1.0 + sin(_Time.y * 6.0)) * 0.5;
                splitAmount *= 1.0 + sin(_Time.y * 16.0) * 0.5;
                splitAmount *= 1.0 + sin(_Time.y * 19.0) * 0.5;
                splitAmount *= 1.0 + sin(_Time.y * 27.0) * 0.5;
                splitAmount = pow(splitAmount,_Amplitude);
                splitAmount *= (0.05 * _Amount);
                
                float2 uv = i.uv;
                float r = tex2D(_MainTex,uv + float2(splitAmount,splitAmount)).r;
                float g = tex2D(_MainTex,uv + float2(0.0,0.0)).g;
                float b = tex2D(_MainTex,uv + float2(-splitAmount,-splitAmount)).b;

                float3 finalColor = float3(r,g,b);
                finalColor *= (1.0 - splitAmount * 0.5);
                
                return float4(finalColor,1.0);
            }
            ENDCG
        }
    }
}
