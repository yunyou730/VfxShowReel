Shader "Ayy/Audio4"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AudioTex("Audio Texture",2D) = "white" {}
        _TestRange("Test Range",Range(1.0,100.0)) = 5.0
        //_BaseLine("Base Line",Range(0.0,0.5)) = 0.5
        
        _WaveFreq("Wave Freq",Range(0.0,100.0)) = 1.0
        _WaveMoveSpeed("Wave Speed",Range(0.0,20.0)) = 1.0
        _WaveScale("Wave Scale",Range(0.0,3.0)) = 1.0
        
        _Toggle("Toggle Sine",Range(0.0,1.0)) = 0.0
        
        _SinFreq("Sine Freq",Range(0.0,100.0)) = 10.0
        _CosFreq("Cos Freq",Range(0.0,100.0)) = 1.0
        
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
            
            sampler2D _MainTex;
            float4 _MainTex_ST;            

            sampler2D _AudioTex;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }
            
        ENDHLSL
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            float _TestRange;
            float _SampleCount;
            
            //float _BaseLine;
            float _WaveFreq;
            float _WaveScale;
            float _WaveMoveSpeed;

            float _Toggle;

            float _SinFreq;
            float _CosFreq;
            
            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float3 col = float3(0.0,0.0,0.0);

                float time = _Time.y;

                const float CNT = 8.0;
                for (float i = 0.0;i < CNT;i += 1.0)
                {
                    float fft = tex2D(_AudioTex,float2(i / (CNT * 5.0),0.0)).r;
                    fft *= _TestRange;
                    
                    float x = uv.x;
                    
                    float sinv = sin(x * _SinFreq + time + (i + 1.0) / CNT * 2.0);
                    float cosv = cos(x * _CosFreq);
                    float v = sinv * cosv * fft;


                    float factor = (i + 1.0) / CNT;
                    //factor = clamp(factor,0.0,0.1);
                    v = v * factor + 0.5;
                    
                    float t = 1.0 - smoothstep(0.0,0.01,abs(uv.y - v));
                    //t *= (i + 1.0) / CNT;

                    col += float3(i / 5.0,0.5,1.75) * t * 3.0 / CNT;
                }
                return float4(col,1.0);
                
            }
                
            
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
    