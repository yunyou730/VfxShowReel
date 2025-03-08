Shader "Ayy/GPUMeshInstance"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                //uint instanceId : SV_InstanceID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v,uint instanceId : SV_InstanceID)
            {
                v2f o;

                //v.vertex.x += v.instanceId * 1.0;
//                v.vertex.y += v.instanceId * 0.3;

                //v.vertex += float4(instanceId,instanceId,0.0,0.0);

                
                //o.vertex = UnityObjectToClipPos(v.vertex);

                float4 worldPos = mul(UNITY_MATRIX_M,v.vertex);
                worldPos.xy += float2(instanceId,instanceId);
                o.vertex = mul(unity_MatrixVP,worldPos);
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col = float4(0.0,0.3,0.8,1.0);
                return col;
            }
            ENDCG
        }
    }
}
