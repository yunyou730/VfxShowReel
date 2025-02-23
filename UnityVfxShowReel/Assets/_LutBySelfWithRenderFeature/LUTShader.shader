Shader "Ayy/LUTShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 UV : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float4 UV : TEXCOORD0;
            };

            sampler2D _MainTex;
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.UV = IN.UV;
                
                return OUT;
            }
            
            float4 frag(Varyings varyings) : SV_Target
            {
                float4 col = tex2D(_MainTex,varyings.UV);
                col *= float4(1.0,1.0,0.0,1.0);
                return col;
            }
            ENDHLSL
        }
    }
}


