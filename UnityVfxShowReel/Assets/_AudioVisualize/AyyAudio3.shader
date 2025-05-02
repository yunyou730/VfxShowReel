Shader "Ayy/Audio3"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AudioTex("Audio Texture",2D) = "white" {}
        _TestRange("Test Range",Range(1.0,1000.0)) = 7.0
        _TimeScale("Time Scale",Range(1.0,20.0)) = 2.0
        _BaseLine("Base Line",Range(0.0,1.414)) = 1.0
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
            float _SampleCount;

            void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
            {
                float2 delta = UV - Center;
                float radius = length(delta) * 2 * RadialScale;
                float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
                Out = float2(radius, angle);
            }
            
            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv = uv * 2.0 - 1.0;

                float sdf = length(uv);

                float2 nuv = normalize(uv);

                float angle = atan2(uv.y, uv.x) * 1.0/6.28; // [-1,+1]
                angle = (angle + 1.0) * 0.5;    // [0,1]

                float fft = tex2D(_AudioTex,float2(angle,0.0)).r * _TestRange;

                float edge = _BaseLine + fft;// + sin(angle);
                float v = step(sdf,edge);
                
                return float4(v,v,v,1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
    