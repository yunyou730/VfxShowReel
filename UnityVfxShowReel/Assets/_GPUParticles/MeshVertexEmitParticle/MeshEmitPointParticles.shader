Shader "Ayy/MeshEmitPointParticles"
{
    Properties
    {
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
                float active : COLOR1;
            };

            // Particle's data, shared with the compute shader
            struct Particle
            {
                float3 Position;
                float3 Velocity;
                float Active;
                float ElapsedTime;
                float LifeTime;
                float3 Color1;
                float3 Color2;
            };
            StructuredBuffer<Particle> Particles;
            
            

            // Vertex shader
            PS_INPUT vert(appdata inData)
            {
                //PS_INPUT output = (PS_INPUT)0;
                PS_INPUT output;
                Particle particle = Particles[inData.instanceId];

                // Color
                float progress = 0.0f;
                if (particle.LifeTime > 0.0f)
                {
                    progress = particle.ElapsedTime / particle.LifeTime;
                    progress = clamp(progress,0.0,1.0);
                }
                float alpha = 1.0 - progress;
                float3 currentColor = lerp(particle.Color1,particle.Color2,progress);
                output.color = float4(currentColor,alpha);
                
                // Position
                output.position = UnityObjectToClipPos(float4(particle.Position, 1.0f));

                // Point Size. Mac need this attribute
                output.pointSize = 1.0f;
                output.active = particle.Active;
                

                return output;
            }

            // Pixel shader
            float4 frag(PS_INPUT i) : COLOR
            {
                if (i.active < 0.5)
                    discard;

                
                return i.color;
            }

            ENDCG
        }
    }

    Fallback Off

}
