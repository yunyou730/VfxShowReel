Shader "Ayy/ShadowReceiver"
{
    Properties
    {
        _BaseColor("Base Color",Color) = (1,1,1,1)
        
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        
        HLSLINCLUDE
        #define _MAIN_LIGHT_SHADOWS
        #define _MAIN_LIGHT_SHADOWS_CASCADE
        #define _SHADOW_SOFT
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
        struct Attributes
        {
            float4 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS :SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normalWS : TEXCOORD1;
            float4 positionOS : TEXCOORD2;
        };

        CBUFFER_START(UnityPerMaterial)
        half4 _BaseColor;
        CBUFFER_END
        

        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS,true);
            OUT.uv = IN.uv;
            OUT.positionOS = IN.positionOS;
            return OUT;
        }

        struct CustomLightingData
        {
            
        };

        float3 CalculateCustomLighting(CustomLightingData d)
        {
            float3 color = float3(0.0,0.0,0.0);

            return color;
        }
        
        half4 frag(Varyings IN) : SV_Target
        {
            float3 normalWS = IN.normalWS;
            float shadowAtten = GetMainLight().shadowAttenuation;


            float4 shadowCoord = 0.0;
            float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
            #if SHADOW_SCREEN
                shadowCoord = ComputeScreenPos(IN.positionHCS)
            #else
                shadowCoord = TransformWorldToShadowCoord(positionWS);
            #endif
            
            Light mainLight = GetMainLight(shadowCoord,positionWS,1);
            float shadowAttenuation = mainLight.shadowAttenuation;

            float3 resultColor = _BaseColor.rgb * shadowAttenuation;
            
            return float4(resultColor,1.0);
            return float4(shadowAttenuation,shadowAttenuation,shadowAttenuation,1.0);
        }
        
        ENDHLSL
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            ENDHLSL
        }
    }
}