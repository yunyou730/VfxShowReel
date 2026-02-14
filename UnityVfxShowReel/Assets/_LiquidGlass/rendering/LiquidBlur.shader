
Shader "Ayy/LiquidBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        ZTest Always Cull Off ZWrite Off
        
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _KernelSize;
            float4 _InputTextureSize;
            //sampler2D _OriginTexture;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #if UNITY_UV_STARTS_AT_TOP
                    o.uv.y = 1.0 - o.uv.y;
                #endif
                if (_ProjectionParams.x < 0.0)
                {
                    o.uv.y = 1.0 - o.uv.y;
                }
                return o;
            }
            float4 sampleTexture(sampler2D tex,float2 uv)
            {
                uv = clamp(uv,0.0,1.0);
                return tex2D(tex,uv);
            }
            
        ENDHLSL
        

        // Horizontal blur
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            half4 frag (v2f i) : SV_Target
            {
                float4 color = float4(0.0,0.0,0.0,0.0);
                float totalWeight = 0.0;
                
                float2 texelSize = float2(1.0/_InputTextureSize.x,1.0/_InputTextureSize.y);
                for (int x = -_KernelSize; x <= _KernelSize; ++x)
                {
                    float weight = exp(-(x * x) / (2.0 * _KernelSize * _KernelSize));
                    color += sampleTexture(_MainTex, i.uv + float2(x, 0) * texelSize) * weight;
                    totalWeight += weight;
                }
                return color / totalWeight;
            }
            ENDHLSL
        }

        // Vertical blur 
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            half4 frag (v2f i) : SV_Target
            {
                float4 color = float4(0,0,0,0);
                float totalWeight = 0.0;
                float2 texelSize = float2(1.0/_InputTextureSize.x,1.0/_InputTextureSize.y);
                for (int y = -_KernelSize; y <= _KernelSize; ++y)
                {
                    float weight = exp(-(y * y) / (2.0 * _KernelSize * _KernelSize));
                    color += sampleTexture(_MainTex, i.uv + float2(0, y) * texelSize) * weight;
                    totalWeight += weight;
                }
                
                return color / totalWeight;
            }
            ENDHLSL
        }

        // blit to screen
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            half4 frag (v2f i) : SV_Target
            {
                float4 blurColor = sampleTexture(_MainTex,i.uv);
                return blurColor;
            }
            ENDHLSL
        }
    }
    //FallBack "Diffuse"
}
