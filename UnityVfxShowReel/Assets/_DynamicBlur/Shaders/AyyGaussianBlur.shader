Shader "Ayy/GaussianBlur"
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

            sampler2D _BlitTexture;
            
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
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            int _kernelSize;
            float4 _screenSize;
            
            half4 frag (v2f i) : SV_Target
            {
                float4 color = float4(0.0,0.0,0.0,0.0);
                float totalWeight = 0.0;
                
                float2 texelSize = float2(1.0/_screenSize.x,1.0/_screenSize.y);
                for (int x = -_kernelSize; x <= _kernelSize; ++x)
                {
                    float weight = exp(-(x * x) / (2.0 * _kernelSize * _kernelSize));
                    //color += sampleTexture(_BlitTexture, i.uv + float2(x, 0) * texelSize) * weight;
                    color += sampleTexture(_MainTex, i.uv + float2(x, 0) * texelSize) * weight;
                    totalWeight += weight;
                }
                return color / totalWeight;
            }
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            int _kernelSize;
            float4 _screenSize;

            
            half4 frag (v2f i) : SV_Target
            {
                float4 color = float4(0,0,0,0);
                float totalWeight = 0.0;

                float2 texelSize = float2(1.0/_screenSize.x,1.0/_screenSize.y);
                for (int y = -_kernelSize; y <= _kernelSize; ++y)
                {
                    float weight = exp(-(y * y) / (2.0 * _kernelSize * _kernelSize));
                    //color += sampleTexture(_BlitTexture, i.uv + float2(0, y) * texelSize) * weight;
                    color += sampleTexture(_MainTex, i.uv + float2(0, y) * texelSize) * weight;
                    totalWeight += weight;
                }
                
                return color / totalWeight;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
    