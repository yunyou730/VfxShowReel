Shader "Ayy/MeshEmitMeshParticles"
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
            Blend SrcAlpha OneMinusSrcAlpha
            //Blend Off
            //ZWrite On
            //ZTest Less
            ZTest Off
            //ZTest Always
            ZWrite Off

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

            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Scale;
            

            // Pixel shader input
            struct PS_INPUT
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float pointSize : PSIZE;
                float active : COLOR1;
                float3 particleWorldPos : COLOR2;
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
                float3 vertexLocalPosition = inData.vertex.xyz * _Scale;
                float3 particleWorldPosition = particle.Position;
                float3 vertexWorldPosition = vertexLocalPosition + particleWorldPosition;
                output.position = UnityWorldToClipPos(float4(vertexWorldPosition,1.0));
                output.particleWorldPos = particleWorldPosition;

                // Point Size. Mac need this attribute
                output.pointSize = 1.0f;
                output.active = particle.Active;
                
                output.uv = inData.uv;

                return output;
            }

            float remap(float lowBound,float highBound,float value)
            {
                return clamp((value - lowBound) / (highBound - lowBound),0.0,1.0);
            }            

            // Pixel shader
            float4 frag(PS_INPUT i) : COLOR
            {
                if (i.active < 0.5)
                    discard;

                float4 texColor = tex2D(_MainTex,i.uv);
                //texColor.rgb = texColor.rgb * texColor.a;
                //texColor.r

                float r = remap(-5.0,5.0,i.particleWorldPos.x);
                float g = remap(-5.0,5.0,i.particleWorldPos.y);
                float b = remap(-5.0,10.0,i.particleWorldPos.z);

                float3 col = float3(r,g,b); 
                return float4(texColor.rgb * col.rgb,texColor.a * i.color.a);
                
                //return float4(texColor.rgb * i.color.rgb,texColor.a * i.color.a);
                //return texColor * i.color;
                return float4(i.uv,0.0,1.0);
                return i.color;
            }
            ENDCG
        }
    }

    Fallback Off

}
