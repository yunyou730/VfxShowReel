Shader "ayy/ObjectWithOutlineForSkeletonAnim"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Map", 2D) = "white" {}
        _MaskObjectId("Mask Object Id",Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry"
        }
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
         
        CBUFFER_START(UnityPerMaterial)
        float4 _BaseColor;
        float4 _BaseMap_ST;
        CBUFFER_END
        
        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);
        
        struct Attributes
        {
            float4 positionOS   : POSITION;
            float2 uv           : TEXCOORD0;
            float3 normalOS     : NORMAL;
        };

        
        struct Varyings
        {
            float4 positionHCS  : SV_POSITION;
            float2 uv           : TEXCOORD0;
            float3 normalWS     : TEXCOORD1;
            float3 positionWS   : TEXCOORD2;
        };

        Varyings VertCommon(Attributes input)
        {
            Varyings output;
            output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
            output.positionHCS = TransformWorldToHClip(output.positionWS);
            output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
            output.normalWS = TransformObjectToWorldNormal(input.normalOS);
            return output;
        }
        ENDHLSL


        Pass // 正常游戏内展示的 Pass
        {
            Name "Game"
            Tags
            {
            //    "LightMode" = "Ayy_OutlineCover"
            }            
            HLSLPROGRAM
            #pragma vertex VertCommon
            #pragma fragment Frag
            half4 Frag(Varyings input) : SV_Target
            {
                half4 baseMapColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                return float4(baseMapColor.rgb,1.0);
            }
            ENDHLSL
        }

        Pass // 正常游戏内展示的 Pass
        {
            Name "Game"
            Tags
            {
                "LightMode" = "Ayy_OutlineCover"
            }            
            HLSLPROGRAM
            #pragma vertex VertCommon
            #pragma fragment Frag
            half4 Frag(Varyings input) : SV_Target
            {
                half4 baseMapColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                return float4(baseMapColor.rgb,1.0);
            }
            ENDHLSL
        }

        Pass //  为了让编辑起能显示、能计算阴影 , 设置的 "DepthOnly" Pass
        {
            Name "ForSceneWindowAndDepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
            HLSLPROGRAM
            #pragma vertex VertCommon
            #pragma fragment Frag
            half4 Frag(Varyings input) : SV_Target
            {
                return float4(1.0,1.0,1.0,1.0);
            }
            ENDHLSL
        }

        Pass     // 在自定义的 RenderFeature 里生效, 用于给 3D物体做 outline 描边准备的 mask图 pass
        {
            ZTest Off
            ZWrite Off
            Cull Back
            
            Name "ForOutlineMask"
            Tags
            {
                "LightMode" = "Ayy_OutlineMask"
            }
            HLSLPROGRAM
            #pragma vertex VertCommon
            #pragma fragment Frag

            float _MaskObjectId;
            half4 Frag(Varyings input) : SV_Target
            {
                return float4(1.0,_MaskObjectId,1.0,1.0);
            }
            ENDHLSL            
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
