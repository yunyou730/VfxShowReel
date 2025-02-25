Shader "Ayy/LUTShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LUT ("LUT",2D) = "white" {}
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
            sampler2D _LUT;
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.UV = IN.UV;
                return OUT;
            }


            float2 GetUV(float b,float r,float g)
            {
                int row = b / 8.0;
                int col = b - row * 8.0;
                float u = float(col) * 1.0 / 8.0;
                float v = 1.0 - row * 1.0/8.0;  // @miao @todo
                
                // 大 Grid 的左上角
                float2 base = float2(u,v);       
            
                // 小 cell 的左上角
                float2 result = base + r * float2(1.0/512.0,0.0) + g * float2(0.0,-1.0/512.0);  
            
                // 小 cell 的中心点 
                const float2 cellCenterOffset = float2(1.0/512.0,-1.0/512.0) * 0.5;
                return result + cellCenterOffset;
            }
            
            float4 frag(Varyings varyings) : SV_Target
            {
                float2 uv = varyings.UV;
                
                float4 col = tex2D(_MainTex,uv);

                //col = float4(0,0,0,1);
                // col = float4(1.0,1.0,1.0,1.0);  // @miao @test
                col = clamp(col,float4(0,0,0,0),float4(1,1,1,1));
            
                float blueIndex = col.b * 63.0;     // blue map to [0,63]
                float redIndex = col.r * 63.0;      // red map to [0,63]
                float greenIndex = col.g * 63.0;    // green map to [0,63];
                
                uv = GetUV(floor(blueIndex),redIndex,greenIndex);
                float4 col1 = tex2D(_LUT,uv);

                // if (uv.x >= 1.0 || uv.x <= 0.0 || uv.y >= 1.0 || uv.y <= 0.0)
                // {
                //     col1 = float4(1.0,0.0,1.0,1.0);
                // }
            
                uv = GetUV(ceil(blueIndex),redIndex,greenIndex);
                float4 col2 = tex2D(_LUT,uv);
                return float4(lerp(col1,col2,frac(blueIndex)).rgb,col.a);
            }


            ENDHLSL
        }
    }
}


