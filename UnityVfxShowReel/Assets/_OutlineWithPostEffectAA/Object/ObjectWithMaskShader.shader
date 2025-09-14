Shader "ayy/ObjectWithMask"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Map", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        
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
        half _Metallic;
        half _Smoothness;
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

        half4 FragInGame(Varyings input) : SV_Target
        {
            half4 baseMapColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
            half3 normalWS = normalize(input.normalWS);
            Light mainLight = GetMainLight();
            
            // lambert light model
            half3 diffuse = baseMapColor.rgb * mainLight.color * max(dot(normalWS, mainLight.direction), 0.0);
            half3 ambient = baseMapColor.rgb * SampleSH(normalWS);
            
            // specular: blinn-phong
            half3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);
            half3 halfDir = normalize(mainLight.direction + viewDir);
            half specularTerm = pow(max(dot(normalWS, halfDir), 0.0), _Smoothness * 100.0);
            half3 specular = mainLight.color * specularTerm * _Metallic;

            // combine lambert & blinn-phong
            half3 finalColor = diffuse + ambient + specular;
            
            return half4(finalColor, baseMapColor.a);
        }        
        ENDHLSL

        Pass // 正常游戏内展示的 Pass
        {
            Name "Game"
            HLSLPROGRAM
            #pragma vertex VertCommon
            #pragma fragment FragInGame
            ENDHLSL
        }

        Pass // 描边之后,覆盖在最上层的 pass 
        {
            Name "Game"
            Tags
            {
                "LightMode" = "Ayy_OutlineCover"
            }            
            HLSLPROGRAM
            #pragma vertex VertCommon
            #pragma fragment FragInGame
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
