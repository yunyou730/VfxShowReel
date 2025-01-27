Shader "Ayy/TessellationWithHeightmap"
{
    Properties
    {
        _TessEdgeFactor("Tess Edge Factor",Vector) = (1,1,1,1)
        _TessInsideFactor("Tess Inside Factor",Float) = 1.0
        _Heightmap("Height map",2D) = "white"
        _HeightOffset("Height Offset",Float) = 5.0
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            
            #pragma target 5.0  // 5.0 required for tessellation
            
            #pragma vertex vert
            #pragma hull Hull
            #pragma domain Domain
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS : NORMAL;
                float2 UV : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessellationControlPoint
            {
                float3 positionWS : INTERNALTESSPOS;
                float3 normalWS : NORMAL;
                float2 UV : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
            // Tessellation factors for Hull PatchConstantFunction 
            float4 _TessEdgeFactor;
            float _TessInsideFactor;
            
            //sampler2D _Heightmap;
            TEXTURE2D(_Heightmap);
            SAMPLER(sampler_Heightmap);

            float _HeightOffset; 
            CBUFFER_END
            
            TessellationControlPoint vert(Attributes input)
            {
                TessellationControlPoint output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input,output);

                VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionWS = posnInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.UV = input.UV;
                
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
                f.edge[0] = _TessEdgeFactor.x;  // 1.0 by default
                f.edge[1] = _TessEdgeFactor.y;  // 1.0 by default
                f.edge[2] = _TessEdgeFactor.z;  // 1.0 by default
                f.inside = _TessInsideFactor;   // 1.0 by default
                return f;
            }
            
            struct Interpolators
            {
                float2 UV : TEXCOORD0;
                float3 normalWS : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
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
                float2 UV = BARYCENTRIC_INTERPOLATE(UV);


                // @miao @todo
                // 采样 高度图, 做 顶点偏移
                float4 heightColor = SAMPLE_TEXTURE2D_LOD(_Heightmap,sampler_Heightmap,UV,0);
                positionWS += float3(0,heightColor.r * _HeightOffset,0);
                
                
                output.positionCS = TransformWorldToHClip(positionWS);
                output.normalWS = normalWS;
                output.positionWS = positionWS;
                output.UV = UV;
                
                return output;
            }
            
            half4 frag(Interpolators input) : SV_Target
            {
                float4 col = float4(1,1,1,1);
                col.r = input.positionWS.y / _HeightOffset;
                
                // if (input.positionWS.y >= 3.0)
                // {
                //     col = float4(1,1,1,1);
                // }
                // else
                // {
                //     col = float4(0,0.5,0,1);
                // }

                
                //return half4(input.UV.x,input.UV.y,0.0,1.0);
                return col;
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