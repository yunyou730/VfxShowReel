Shader "Ayy/GPUParticlesSample1"
{

    Properties
    {
        _ColorLow("Color Slow Speed", Color) = (0, 0, 0.5, 1)
        _ColorHigh("Color High Speed", Color) = (1, 0, 0, 1)
        _HighSpeedValue("High speed Value", Range(0, 50)) = 25
    }

    SubShader
    {
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            //#pragma target 5.0

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Particle's data
            struct Particle
            {
                float2 position;
                float2 velocity;
            };

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
                float4 color : COLOR;
                float pointSize : PSIZE; 
            };

            // Particle's data, shared with the compute shader
            StructuredBuffer<Particle> Particles;
            float PointSize;

            // Properties variables
            uniform float4 _ColorLow;
            uniform float4 _ColorHigh;
            uniform float _HighSpeedValue;

            // Vertex shader
            //PS_INPUT vert(uint vertexId : SV_VertexID, uint instanceId : SV_InstanceID)
            PS_INPUT vert(appdata inData)
            {
                PS_INPUT output = (PS_INPUT)0;

                // Color
                float speed = length(Particles[inData.instanceId].velocity);
                float lerpValue = clamp(speed / _HighSpeedValue, 0.0f, 1.0f);
                output.color = lerp(_ColorLow, _ColorHigh, lerpValue);

                // @miao @todo
                //output.color = float4(1.0,0.0,0.0,1.0);
                
                // Position
                //output.position = UnityObjectToClipPos(float4(Particles[instanceId].position, 1.0f));

                float3 pos3d = inData.vertex.xyz + float3(Particles[inData.instanceId].position.xy,0.0) * PointSize;
                float4 localPosition = float4(pos3d, 1.0f);
                output.position = UnityObjectToClipPos(localPosition);
                //output.position = float4(0.0,0.0,0.0,1.0);

                output.pointSize = PointSize;

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
