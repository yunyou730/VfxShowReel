Shader "Ayy/GrassWithGeoTess"
{
    Properties
    { }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Cull Off
            
            HLSLPROGRAM
            
            //#pragma target 4.0 
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geo


            // #pragma prefer_hlslcc gles
            // #pragma exclude_renderers d3d11_9x
            // #pragma target 4.0
            // #pragma require geometry
            // #pragma multi_compile_instancing            

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct Varyings
            {
                float4 positionOS : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            
            struct GeometryOutput
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };
            
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionOS = IN.positionOS;
                OUT.normal = IN.normal;
                OUT.tangent = IN.tangent;
                return OUT;
            }


            GeometryOutput VertexOutput(float3 pos)
            {
                GeometryOutput o;
                o.pos = TransformObjectToHClip(pos);
                o.color = float4(0,1,0,1);
                return o;
            }

            [maxvertexcount(6)]
            void geo(triangle Varyings IN[3],inout TriangleStream<GeometryOutput> triStream)            
            {
                float3 pos = IN[0].positionOS;
                float3 vNormal = IN[0].normal;
                float4 vTangent = IN[0].tangent;
                
                // vTangent.w = -1 or 1, determine bitangent direction. Reference below link
                // https://discussions.unity.com/t/what-is-tangent-w-how-to-know-whether-its-1-or-1-tangent-w-vs-unity_worldtransformparams-w/662979/1
                float3 vBinormal = cross(vNormal,vTangent) * vTangent.w;
                
                float3x3 tangentToLocal = float3x3(
                    vTangent.x,vBinormal.x,vNormal.x,
                    vTangent.y,vBinormal.y,vNormal.y,
                    vTangent.z,vBinormal.z,vNormal.z
                );
                
                triStream.Append(VertexOutput(pos + mul(tangentToLocal,float3(0.5,0,0))));
                triStream.Append(VertexOutput(pos + mul(tangentToLocal,float3(-0.5,0,0))));
                triStream.Append(VertexOutput(pos + mul(tangentToLocal,float3(0,0,1))));    // z as up in Tangent Space
                triStream.RestartStrip();
            }
            
            half4 frag(GeometryOutput IN) : SV_Target
            {
                // half4 customColor = half4(0.0, 0.0, 1.0, 1);
                // return customColor;

                return IN.color;
            }
            ENDHLSL
        }
    }
}