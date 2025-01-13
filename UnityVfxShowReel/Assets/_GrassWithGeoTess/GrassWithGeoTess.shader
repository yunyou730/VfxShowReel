Shader "Ayy/GrassWithGeoTess"
{
    Properties
    { }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
         
            HLSLPROGRAM

            //#pragma target 4.0 

         
            #pragma vertex vert
            #pragma fragment frag
            //#pragma geometry geo


            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 4.0
            #pragma require geometry
            #pragma multi_compile_instancing            

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                //float4 positionOS : SV_POSITION;
                float4 positionHCS  : SV_POSITION;
            };
            

            struct GeometryOutput
            {
                float4 pos : SV_POSITION;
            };
            
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            [maxvertexcount(3)]
            void geo(triangle Varyings IN[3],inout TriangleStream<GeometryOutput> triStream)            
            {
                //void geo(triangle float4 IN[3] : SV_POSITION,inout TriangleStream<GeometryOutput> triStream)
                GeometryOutput o;
                
                o.pos = float4(0.5, 0, 0, 1);
                triStream.Append(o);

                o.pos = float4(-0.5, 0, 0, 1);
                triStream.Append(o);

                o.pos = float4(0, 1, 0, 1);
                triStream.Append(o);
            }
            
            half4 frag() : SV_Target
            {
                half4 customColor = half4(1.5, 1.0, 0, 1);
                return customColor;
            }
            ENDHLSL
        }
    }
}