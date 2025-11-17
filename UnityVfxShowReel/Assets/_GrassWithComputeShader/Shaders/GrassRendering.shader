Shader "Ayy/GrassRendering"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Scale ("Scale",Range(0,2)) = 1.0
    }

    SubShader
    {
        Pass
        {
            //Blend SrcAlpha OneMinusSrcAlpha
            Blend Off
            //ZWrite On
            //ZTest Less
            ZTest Off
            //ZTest Always
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma target 5.0

            #pragma vertex vert
            #pragma fragment frag
            

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint vertexId : SV_VertexID;
                uint instanceId : SV_InstanceID;
            };
            
            // Pixel shader input
            struct PS_INPUT
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float instanceID : COLOR0;
            };

            struct Grass
            {
                float3 Position;
            };
            StructuredBuffer<Grass> _ayy_GrassData;

            float4x4 _ayy_MatrixIdentity;

            // Vertex shader
            PS_INPUT vert(appdata inData)
            {
                PS_INPUT output;
                Grass grass = _ayy_GrassData[inData.instanceId];
                
                // Position
                float3 vertexLocalPosition = inData.vertex.xyz;
                float3 vertexWorldPosition = vertexLocalPosition + grass.Position;// + float3(inData.instanceId - 10.0,0.0,0.0);
                //float3 vertexWorldPosition = vertexLocalPosition * 10.0f;
                output.position = UnityWorldToClipPos(vertexWorldPosition);
                //output.position = mul(_ayy_MatrixIdentity, float4(vertexLocalPosition, 1.0));
                output.uv = inData.uv;

                output.instanceID = inData.instanceId / 128.0;

                return output;
            }

            float4 frag(PS_INPUT i) : COLOR
            {
                return float4(i.instanceID,0.0,0.0,1.0);
            }
            ENDCG
        }
    }

    Fallback Off

}
