Shader "Ayy/Audio5"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AudioTex("Audio Texture",2D) = "white" {}
        _TestRange("Test Range",Range(1.0,200.0)) = 5.0
        
        _Bands("Bands in X",Range(10.0,50.0)) = 30.0
        _Segs("Segs in Y",Range(10.0,50.0)) = 40.0
        
        _UvMinBound("uv min bound",Range(0.0,1.0)) = 0.35
        _UvMaxBound("uv max bound",Range(0.0,1.0)) = 0.5
        
        _Toggle("Enable Remap Sample Range",Range(0.0,1.0)) = 0.0
        _RemapScale("Remap Scale",Range(0.0,1.0)) = 1.0
        _RemapOffset("Remap Offset",Range(0.0,1.0)) = 0.0
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

            float _Bands = 30.0;
            float _Segs = 40.0;

            float _UvMinBound;
            float _UvMaxBound;

            float _Toggle;
            float _RemapScale;
            float _RemapOffset;

            // reference https://www.shadertoy.com/view/Mlj3WV
            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float2 p;
                p.x = floor(uv.x * _Bands) / _Bands;
                p.y = floor(uv.y * _Segs) / _Segs;


                float sampleX = p.x;        // [0,1]
                if (_Toggle > 0.5)
                {
                    sampleX = sampleX * 2.0 - 1.0;  // [-1,+1]
                    sampleX *= _RemapScale; // [-0.8,+0.8]
                    sampleX -= _RemapOffset;
                    sampleX = sampleX * 0.5 + 0.5;  // remap back to [0,1] but scaled, resut in [0.1,0.9]   
                }
                
                float fft = tex2D(_AudioTex,float2(sampleX,0.0)).x * _TestRange;
                //fft = clamp(fft,0.0,0.8);

                
                float3 color = lerp(float3(0.0,2.0,0.0),float3(2.0,0.0,0.0),sqrt(uv.y));

                float mask = (p.y < fft) ? 1.0 : 0.1;
                // shape
                float2 d = frac((uv - p) * float2(_Bands,_Segs)) - 0.5;
                //float2 d = frac(uv * float2(_Bands,_Segs)) - 0.5;
                float led = smoothstep(_UvMaxBound,_UvMinBound,abs(d.x)) * smoothstep(_UvMaxBound,_UvMinBound,abs(d.y));
                float3 ledColor = led * color * mask;
                
                //return float4(d,0.0,1.0);
                //float time = _Time.y;
                return float4(ledColor,1.0);

                //return float4(sampleX,0.0,0.0,1.0);
            }
                
            
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
    