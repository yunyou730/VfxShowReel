Shader "Ayy/OutlineWithPostEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("OutlineColor",Color) = (1,0,0,1)
        
        [Toggle(SWITCH_MASK_ORIGIN)] _SwitchMaskAndOrigin ("SWITCH_MASK_ORIGIN", Int) = 0
        
        // dilate
        _DilateKernelSize ("Dilate Kernel Size",Range(0,20)) = 3 // 膨胀 卷集核 大小 
        _EnableDilate("Enable Dilate",Range(0,1)) = 1.0
        
        // blur
        _BlurKernelSize ("Blur Kernel Size",Range(0,20)) = 5    // 模糊 卷积核大小
        
        // Mask
        _MaskStrength("MaskStrength",Range(0,10)) = 1.0
        _MaskLower("MaskLower",Range(0,1)) = 0.0
        _MaskInc("MaskInc",Range(0,1)) = 0.1
        
        // Distortion
        _DistortionNoiseScale("Distortion Noise Scale",Range(-100,100)) = 20
        _DistortionStrengthX("Distortion Strength X",Range(-1.0,1.0)) = 0.1
        _DistortionStrengthY("Distortion Strength Y",Range(-1.0,1.0)) = 0.1
        _DistortionSpeedX("Distortion Speed X",Range(-1,1)) = 1.0
        _DistortionSpeedY("Distortion Speed Y",Range(-1,1)) = 1.0
        
        _ColorFrom ("Color From",Color) = (1,0,0,1)
        _ColorTo ("Color To",Color) = (1,1,0,1)
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


 // simple noise begin
          float noise_randomValue(float2 uv)
          {
             return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
          }

          float noise_interpolate(float a, float b, float t)
          {
              return (1.0 - t) * a + (t * b);
          }
          
          float noise_valueNoise (float2 uv)
          {
              float2 i = floor(uv);
              float2 f = frac(uv);
              f = f * f * (3.0 - 2.0 * f);

              uv = abs(frac(uv) - 0.5);
              float2 c0 = i + float2(0.0, 0.0);
              float2 c1 = i + float2(1.0, 0.0);
              float2 c2 = i + float2(0.0, 1.0);
              float2 c3 = i + float2(1.0, 1.0);
              float r0 = noise_randomValue(c0);
              float r1 = noise_randomValue(c1);
              float r2 = noise_randomValue(c2);
              float r3 = noise_randomValue(c3);

              float bottomOfGrid = noise_interpolate(r0, r1, f.x);
              float topOfGrid = noise_interpolate(r2, r3, f.x);
              float t = noise_interpolate(bottomOfGrid, topOfGrid, f.y);
              return t;
          }

          float simpleNoise(float2 UV, float Scale)
          {
              float t = 0.0;

              float freq = pow(2.0, float(0));
              float amp = pow(0.5, float(3-0));
              t += noise_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

              freq = pow(2.0, float(1));
              amp = pow(0.5, float(3-1));
              t += noise_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

              freq = pow(2.0, float(2));
              amp = pow(0.5, float(3-2));
              t += noise_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

             return t;
          }
          // simple noise end            
            
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
                //return tex2D(_AyyOutlineMask,uv);
                
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
        Pass
        {
            Name "Ayy_Blit"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_OriginTex);

            float4 _OutlineColor;
            
            float _MaskLower;
            float _MaskInc;
            
            float _DistortionNoiseScale;
            float _DistortionStrengthX;
            float _DistortionStrengthY;
            float _DistortionSpeedX;
            float _DistortionSpeedY;

            float4 _ColorFrom;
            float4 _ColorTo;
            
            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 originUV = uv;
                
                // distort uv
                float2 noiseUV = uv;
                noiseUV.x += _Time.y * _DistortionSpeedX;
                noiseUV.y += _Time.y * _DistortionSpeedY;
                float offset = simpleNoise(noiseUV,_DistortionNoiseScale);
                offset = offset * 2.0 - 1.0;

                uv.x += offset * _DistortionStrengthX;
                uv.y += offset * _DistortionStrengthY;
                
                float4 mainColor = SAMPLE_TEXTURE2D_X(_OriginTex,sampler_LinearClamp,originUV);
                float4 maskColor = SAMPLE_TEXTURE2D_X(_MainTex,sampler_LinearClamp,uv);
                
                float lower = clamp(_MaskLower,0.0,1.0);
                float higher = clamp(_MaskLower + _MaskInc,0.0,1.0);
                float mask = smoothstep(lower,higher,maskColor.r);


                float4 outlineColor = lerp(_ColorFrom,_ColorTo,smoothstep(-1.0,1.0,offset));
                
                float4 ret = lerp(mainColor,outlineColor,mask);

                //ret = outlineColor;
                return ret;
            }
            ENDHLSL
        }
    }
    //FallBack "Diffuse"
}
    