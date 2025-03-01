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
        //Tags {"Queue" = "Transparent"}
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma target 5.0

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Particle's data
            struct Particle
            {
                float2 position;
                float2 velocity;
            };

            // Pixel shader input
            struct PS_INPUT
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float pointSize : PSIZE; 
            };

            // Particle's data, shared with the compute shader
            //StructuredBuffer<Particle> Particles;

            // Properties variables
            uniform float4 _ColorLow;
            uniform float4 _ColorHigh;
            uniform float _HighSpeedValue;

            // Vertex shader
            PS_INPUT vert(uint vertexId : SV_VertexID, uint instanceId : SV_InstanceID)
            {
                PS_INPUT output = (PS_INPUT)0;

                // Color
                //float speed = length(Particles[instance_id].velocity);
                //float lerpValue = clamp(speed / _HighSpeedValue, 0.0f, 1.0f);
                //o.color = lerp(_ColorLow, _ColorHigh, lerpValue);

                // @miao @todo
                output.color = float4(1.0,0.0,0.0,1.0);
                
                // Position
                //o.position = UnityObjectToClipPos(float4(Particles[instance_id].position, 0.0f, 1.0f));
                output.position = float4(0.0,0.0,0.0,1.0);

                output.pointSize = 20.0;

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
