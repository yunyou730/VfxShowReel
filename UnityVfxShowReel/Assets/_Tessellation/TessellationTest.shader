Shader "Ayy/TessellationTest"
{
    Properties
    { }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
//            Cull Off
            HLSLPROGRAM
            
            #pragma target 5.0
            
            #pragma vertex vert
            #pragma hull Hull
            #pragma domain Domain
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessellationControlPoint
            {
                float3 positionWS : INTERNALTESSPOS;
                float3 normalWS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            TessellationControlPoint vert(Attributes input)
            {
                TessellationControlPoint output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input,output);

                VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionWS = posnInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                
                return output;
            }

            // The hull function runs once per vertex. You can use it to modify vertex data based on values in the entire triangle
            [domain("tri")] // Signal we are inputting triangles
            [outputcontrolpoints(3)] // triangles have 3 points
            [outputtopology("triangle_cw")] // signal we are outputting triangles, clock wise
            [patchconstantfunc("PatchConstantFunction")] // Register the patch constant function
            [partitioning("integer")] // Select a partitioning mode: integer, fractional_odd, fractional_even, pow2
            TessellationControlPoint Hull(
                InputPatch<TessellationControlPoint,3> patch, // Input triangle
                uint id : SV_OutputControlPointID)  // Vertex index on the triangle
            {
                return patch[id];
            }

            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };
            
            // The patch sontant funciton runs once per triangle , or "patch"
            // It runs in parallel to the hull function
            TessellationFactors PatchConstantFunction(
                InputPatch<TessellationControlPoint,3> patch)
            {
                UNITY_SETUP_INSTANCE_ID(patch[0]);  // setup instancing

                // Calculate tessellation factors
                TessellationFactors f;
                f.edge[0] = 1;
                f.edge[1] = 1;
                f.edge[2] = 1;
                f.inside = 1;
                return f;
            }


            
            struct Interpolators
            {
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float4 positionCS : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #define BARYCENTRIC_INTERPOLATE(fieldName) \
                    patch[0].fieldName * barycentricCoordinates.x + \
                    patch[1].fieldName * barycentricCoordinates.y + \
                    patch[2].fieldName * barycentricCoordinates.z
                    

            [domain("tri")] // singal we are inputting triangles
            Interpolators Domain(
                TessellationFactors factors, // The output of the patch constant function
                OutputPatch<TessellationControlPoint,3> patch, // the input triangle, or "patch",
                float3 barycentricCoordinates: SV_DomainLocation // the barycentric coordinates of the vertex on the triangle
            )
            {
                Interpolators output;
                
                UNITY_SETUP_INSTANCE_ID(patch[0]);
                UNITY_TRANSFER_INSTANCE_ID(patch[0],output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 positionWS = BARYCENTRIC_INTERPOLATE(positionWS);
                float3 normalWS = BARYCENTRIC_INTERPOLATE(normalWS);

                
                output.positionCS = TransformWorldToHClip(positionWS);
                output.normalWS = normalWS;
                output.positionWS = positionWS;
                
                return output;
            }
            
            half4 frag() : SV_Target
            {
                return half4(1.0,0.0,0.0,1.0);
            }

            
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ColorMask R
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LOD_FADE_CROSSFADE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }     
    }
}