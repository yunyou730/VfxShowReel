Shader "Ayy/GPUTriangles"
{
    Properties
    {
    }

    SubShader
    {
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            //#pragma target 5.0

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Custom vertex attribute
            struct CustomVertexAttribute
            {
                float3 position;
            };

            // Pixel shader input
            struct PS_INPUT
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float pointSize : PSIZE; 
            };

            // custom vertex attribute
            StructuredBuffer<CustomVertexAttribute> CustomBufferData;

            // Properties variables
            uniform float4 _ColorLow;
            uniform float4 _ColorHigh;
            uniform float _HighSpeedValue;

            // Vertex shader
            PS_INPUT vert(uint vertexId : SV_VertexID, uint instanceId : SV_InstanceID)
            {
                PS_INPUT output = (PS_INPUT)0;
                // Color
                output.color = float4(1.0,0.0,0.0,1.0);
                // Position
                output.position = UnityObjectToClipPos(float4(CustomBufferData[vertexId].position, 1.0f));
                return output;
            }

            // Pixel shader
            float4 frag(PS_INPUT i) : COLOR
            {
                return i.color;
            }

            ENDCG
        }
    }

    Fallback Off

}
