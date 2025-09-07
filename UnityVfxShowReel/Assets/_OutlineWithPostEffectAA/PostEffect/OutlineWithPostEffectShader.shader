Shader "Ayy/OutlineWithPostEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        [Toggle(SWITCH_MASK_ORIGIN)] _SwitchMaskAndOrigin ("SWITCH_MASK_ORIGIN", Int) = 0
        _DilateKernelSize ("Dilate Kernel Size",Range(0,20)) = 3 // 膨胀 卷集核 大小 
        _EnableDilate("Enable Dilate",Range(0,1)) = 1.0
        
        
        _BlurKernelSize ("Blur Kernel Size",Range(0,20)) = 5    // 模糊 卷积核大小
        
        
    }
    SubShader
    {
        ZTest Always Cull Off ZWrite Off
        
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/DebuggingFullscreen.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            
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
            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #if UNITY_UV_STARTS_AT_TOP
                    o.uv.y = 1.0 - o.uv.y;
                #endif
                if (_ProjectionParams.x < 0.0)
                {
                    o.uv.y = 1.0 - o.uv.y;
                }
                return o;
            }
            
        ENDHLSL
        
        // 把 mask图 膨胀,并做显示
        Pass
        {
            Name "Ayy_Dilate"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _AyyOutlineMask;

            int _SwitchMaskAndOrigin;

            int _DilateKernelSize;
            float _EnableDilate;
            
            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 col = tex2D(_MainTex,uv);
                float4 col2 = tex2D(_AyyOutlineMask,uv);
                
                if (_SwitchMaskAndOrigin == 1)
                {
                    return col;   
                }

                float v = 0.0f;
                int radius = _DilateKernelSize;
                for (int y = -radius;y <= radius;y++)
                {
                    for (int x = -radius;x <= radius;x++)
                    {
                        float2 offset = float2(x,y) * _MainTex_TexelSize.xy;

                        float2 curUV = uv + offset;
                        float gray = tex2D(_AyyOutlineMask,curUV).r;
                        v = max(gray,v);
                    }
                }
                return float4(v,v,v,1.0);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Ayy_Blur_H"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);            

            int _BlurKernelSize;

            float _ScreenWidth;
            float _ScreenHeight;
            
            
            half4 frag (v2f i) : SV_Target
            {
                float4 color = float4(0.0,0.0,0.0,0.0);
                float totalWeight = 0.0;
                
                //float2 texelSize = _ScreenParams.zw;
                float2 texelSize = float2(1.0/_ScreenWidth,1.0/_ScreenHeight);
                for (int x = -_BlurKernelSize; x <= _BlurKernelSize; ++x)
                {
                    float weight = exp(-(x * x) / (2.0 * _BlurKernelSize * _BlurKernelSize));
                    float2 uv = i.uv + float2(x, 0) * texelSize;
                    color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv) * weight;
                    totalWeight += weight;
                }
                return float4((color / totalWeight).rgb,1.0);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Ayy_Blur_V"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);            

            int _BlurKernelSize;

            float _ScreenWidth;
            float _ScreenHeight;
            
            
            half4 frag (v2f i) : SV_Target
            {
                float4 color = float4(0.0,0.0,0.0,0.0);
                float totalWeight = 0.0;
                
                //float2 texelSize = _ScreenParams.zw;
                float2 texelSize = float2(1.0/_ScreenWidth,1.0/_ScreenHeight);
                for (int y = -_BlurKernelSize; y <= _BlurKernelSize; ++y)
                {
                    float weight = exp(-(y * y) / (2.0 * _BlurKernelSize * _BlurKernelSize));
                    float2 uv = i.uv + float2(0, y) * texelSize;
                    color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv) * weight;
                    totalWeight += weight;
                }
                return float4((color / totalWeight).rgb,1.0);
            }
            ENDHLSL
        }
    }
    //FallBack "Diffuse"
}
    