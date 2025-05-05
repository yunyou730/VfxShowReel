Shader "Ayy/Audio2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AudioTex("Audio Texture",2D) = "white" {}
        _TestRange("Test Range",Range(1.0,1000.0)) = 7.0
        _TimeScale("Time Scale",Range(1.0,20.0)) = 2.0
        _Waves("Waves",Range(1.0,20.0)) = 8.0
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
            float _TimeScale;
            float _Waves;
            
            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                uv = uv * 2.0 - 1.0;

                const float _Waves = 8.0;
                float time = _Time.y * _TimeScale;

                float3 color = float3(0.0,0.0,0.0);
                for (float i = 0.0; i < _Waves + 1.0; i += 1.0)
                {
                    float freq = tex2D(_AudioTex, float2(i / _Waves, 0.25)).x * _TestRange;

                    float2 p = uv;
                    p.x += i * 0.04 + freq * 0.03;
                    p.y += sin(p.x * 10.0 + time) * cos(p.x * 2.0) * freq * 0.2 * ((i + 1.0) / _Waves);
                    float intensity = abs(0.01 / p.y) * clamp(freq,0.35,2.0);
                    color += float3(1.0 * intensity * (i / 5.0),0.5 * intensity,1.75 * intensity) * (3.0 / _Waves);
                }
                return float4(color,1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
    