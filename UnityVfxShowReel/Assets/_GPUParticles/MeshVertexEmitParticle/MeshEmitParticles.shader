Shader "Ayy/MeshEmitParticles"
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
            struct Particle
            {
                float3 Position;
                float3 Velocity;
                float Active;
                float ElapsedTime;
                float LifeTime;
            };
            StructuredBuffer<Particle> Particles;

            // Properties variables
            uniform float4 _ColorLow;
            uniform float4 _ColorHigh;
            uniform float _HighSpeedValue;

            // Vertex shader
            PS_INPUT vert(appdata inData)
            {
                PS_INPUT output = (PS_INPUT)0;

                // Color
                // float speed = length(Particles[inData.instanceId].velocity);
                // float lerpValue = clamp(speed / _HighSpeedValue, 0.0f, 1.0f);
                // output.color = lerp(_ColorLow, _ColorHigh, lerpValue);
                output.color = float4(1.0,1.0,0.0,1.0);
                
                
                // Position
                Particle particle = Particles[inData.instanceId];
                output.position = UnityObjectToClipPos(float4(particle.Position, 1.0f));

                // Point Size. Mac need this attribute
                output.pointSize = 1.0f;

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
