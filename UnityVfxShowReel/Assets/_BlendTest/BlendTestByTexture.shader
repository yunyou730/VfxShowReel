Shader "Ayy/BlendTutorial_Texture"
{
    Properties
    {
        _MainTex("Main Tex",2D) = "white" {}
        
        [Enum(UnityEngine.Rendering.BlendMode)]
        _BlendSrc("Blend Source",int) = 0
        
        [Enum(UnityEngine.Rendering.BlendMode)]
        _BlendDst("Blend Dest",int) = 0
        
        [Enum(UnityEngine.Rendering.BlendOp)]
        _BlendOp("Blend OP",int) = 0
    }
    
    SubShader
    {
        Tags {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Pass
        {
            Cull Off
            Blend [_BlendSrc] [_BlendDst]
            BlendOp [_BlendOp]
            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            
            sampler2D _MainTex;
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 textCoord : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 texCoord : TEXCOORD0;
                float4 positionOS : TEXCOORD1;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.texCoord = IN.textCoord;
                OUT.positionOS = IN.positionOS;
                OUT.normal = IN.normal;
                OUT.tangent = IN.tangent;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 col = tex2D(_MainTex,IN.texCoord);
                return col;
                //return half4(col.g,col.g,col.b,col.a);
                //return half4(IN.texCoord.xy,0.0,1.0);
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