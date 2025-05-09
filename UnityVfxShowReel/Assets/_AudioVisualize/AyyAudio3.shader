Shader "Ayy/Audio3"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AudioTex("Audio Texture",2D) = "white" {}
        _TestRange("Test Range",Range(1.0,1000.0)) = 7.0
        _TimeScale("Time Scale",Range(1.0,20.0)) = 2.0
        _BaseLine("Base Line",Range(0.0,1.414)) = 1.0
        _InnerBorder("Inner border",Range(0.0,1.0)) = 0.0
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
            float _BaseLine;
            float _InnerBorder;
            float _SampleCount;


            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0,2.0/3.0,1.0/3.0,3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K .www);
                return c.z * lerp(K.xxx,clamp(p - K.xxx,0.0,1.0),c.y);
            }
            
            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv = uv * 2.0 - 1.0;

                float sdf = length(uv);

                float angle = atan2(uv.y, uv.x) / 6.28; // [-1,+1]
                angle = (angle + 1.0) * 0.5;    // [0,1]

                float fft = tex2D(_AudioTex,float2(angle,0.25)).r * _TestRange;

                float edge = _BaseLine + fft;// + sin(angle);
                float v = step(sdf,edge) * smoothstep(_BaseLine - _InnerBorder,_BaseLine,sdf);

                //v = 1.0 - smoothstep(_BaseLine,edge,sdf);
                //float v = smoothstep(edge,0.1,sdf);

                
                angle = atan2(uv.y,uv.x);
                float hue = (angle + 3.141592653589793) / (2.0 * 3.141592653589793);
                float3 col = float3(hue,1.0,1.0);
                col = hsv2rgb(col);
                col *= v;
                
                return float4(col,1.0);
                
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
    