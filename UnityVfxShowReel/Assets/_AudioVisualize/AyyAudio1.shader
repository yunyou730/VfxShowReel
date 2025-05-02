Shader "Ayy/Audio1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AudioTex("Audio Texture",2D) = "white" {}
        _TestRange("Test Range",Range(1.0,20.0)) = 5.0
        _BaseLine("Base Line",Range(0.0,0.5)) = 0.5
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
            float _BaseLine;
            
            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float fft = tex2D(_AudioTex,float2(uv.x,0.0)).r;
                fft *= _TestRange;      // range 比较大的时候, 能够观察到 fft 的波形


                //float baseLine = sin(uv.x * 0.1 + _Time.y) + _BaseLine;
                float baseLine = _BaseLine;

                float threshold = baseLine + fft;
                float ret = step(uv.y,threshold);
                return float4(ret,ret,ret,1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
    