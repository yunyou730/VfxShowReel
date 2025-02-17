Shader "Ayy/SplitScreen"
{
    Properties
    {
        //_Tex1 ("Tex1", 2D) = "white" {}
        //_Tex2 ("Tex2", 2D) = "white" {}
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

            sampler2D _Tex1;
            float4 _Tex1_ST;

            sampler2D _Tex2;
            float4 _Tex2_ST;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;//TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2;
                
                uv = frac(uv);
                
                fixed4 col1 = tex2D(_Tex1, uv);
                fixed4 col2 = tex2D(_Tex2, uv);

                return col1 + col2;
                //fixed4 col3 = tex2D(_Tex3, uv);
                //fixed4 col4 = tex2D(_Tex4, uv);
//                return col * float4(_Color.rgb,1.0);

                //return col1 + col2 + col3 + col4;
            }
            ENDCG
        }
    }
}
