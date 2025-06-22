Shader "Ayy/Terrain"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _MainColor("Main Color",Color) = (1,1,1,1)
        _GridColor("Main Color",Color) = (1,1,1,1)
        
        _Rows("Rows",Range(1,300)) = 50
        _Cols("Cols",Range(1,300)) = 50
        
        _BorderSizeInUV("Border Size",Range(0.01,0.2)) = 0.1
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

            float _Rows;
            float _Cols;
            float _BorderSizeInUV;
            float4 _MainColor;
            float4 _GridColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            //sampler2D _MainTex;
            //float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv *= float2(_Cols,_Rows);
                uv.x = frac(uv.x);
                uv.y = frac(uv.y);
                //uv.x *= _Cols;
                //uv.y *= _Rows;

                fixed4 col = _MainColor;

                if (uv.x < _BorderSizeInUV || uv.x > 1.0 - _BorderSizeInUV
                    || uv.y < _BorderSizeInUV || uv.y > 1.0 - _BorderSizeInUV)
                {
                    col = _GridColor;
                }
                
                return col;
            }
            ENDCG
        }
    }
}
