Shader "Ayy/GPUTrianglesInstance"
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

           

            // Pixel shader input
            struct PS_INPUT
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float pointSize : PSIZE; 
            };

            
            

            // Properties variables
            uniform float4 _ColorLow;
            uniform float4 _ColorHigh;
            uniform float _HighSpeedValue;

            // Vertex shader
            PS_INPUT vert(uint vertexId : SV_VertexID, uint instanceId : SV_InstanceID)
            {
                PS_INPUT output = (PS_INPUT)0;
                
                float3 base = float3(instanceId,0,0);
                float3 pos = base;
                if (vertexId == 0)
                {
                    pos = base + float3(-0.5,-0.5,0);
                }
                else if (vertexId == 1)
                {
                    pos = base + float3(0.5,-0.5,0);                    
                }
                else if (vertexId == 2)
                {
                    pos = base + float3(0,0.5,0);       
                }
                
                output.position = UnityObjectToClipPos(float4(pos,1.0));
                output.color = float4(1.0,0.0,0.0,1.0);
                
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
