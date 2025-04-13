Shader "Ayy/OldSchoolPlus"
{
    Properties
    {
        _BaseColor("Base Color",Color) = (1,1,1,1)
        
        _EnvUpCol("Env Up Color",Color) = (1,1,1,1)
        _EnvSideCol("Env Side Color",Color) = (1,1,1,1)
        _EnvDownCol("Env Down Color",Color) = (1,1,1,1)
        
        _AO("AO",2D) = "white" {}
        
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        
        HLSLINCLUDE
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
        };

        CBUFFER_START(UnityPerMaterial)
        half4 _BaseColor;
        float4 _EnvUpCol;
        float4 _EnvSideCol;
        float4 _EnvDownCol;
        sampler2D _AO;
        CBUFFER_END
        

        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS,true);
            OUT.uv = IN.uv;
            return OUT;
        }


        float3 getAmblentColor3Dir(float3 normalWS,float2 uv)
        {
            float up = max(normalWS.g,0.0);
            float down = max(-normalWS.g,0.0);
            float side = 1.0 - up - down;            
            
            float3 col = up * _EnvUpCol + down * _EnvDownCol + side * _EnvSideCol;
            float3 ao = tex2D(_AO,uv).rgb;
            col *= ao;
            return col;
        }

        float3 getDiffuseColor(float3 normalWS)
        {
            Light mainLight = GetMainLight();
            float3 mainLightDirWS = normalize(mainLight.direction);
            float lDotN = dot(mainLightDirWS,normalWS);

            return float3(lDotN,lDotN,lDotN);
        }


        //#include "Lighting.cginc"

        half4 frag(Varyings IN) : SV_Target
        {
            //float3 normalWS = getNormalDirection(IN.normal);
            float3 normalWS = IN.normalWS;
            
            
            float3 ambient = getAmblentColor3Dir(normalWS,IN.uv);
            
            //float3 col = tex2D(_AO,IN.uv).rgb;
            //col = pow(col,2.0);

            float shadowAtten = GetMainLight().shadowAttenuation;
            //ambient *= shadowAtten;

            
            return float4(ambient,1.0);
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