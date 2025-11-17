Shader "ayy/Glitch/ImageBlockGlitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
//        _Amplitude("Amplitude",Range(0.1,5.0)) = 1.0
//        _Amount("Amount",Range(0.0,5.0)) = 1.0
        _BlockSize("BlockSize",Range(1,10)) = 5
        _Speed("Speed",Range(0,10)) = 1
        _DisplacementMoveSpeed("DisplacementMoveSpeed",Range(0,5)) = 0.1
        _Strength("Strength",Range(0,3)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            
            float _BlockSize;
            float _Speed;
            float _DisplacementMoveSpeed;
            float _Strength;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

	        float rand1D(float seed)
	        {
		        return frac(sin(seed * 12.9898) * 43758.5453123);
	        }

            float rand2D(float2 p)
            {
                return frac(sin(dot(p,float2(12.9898,78.233))) * 43758.5453);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float displace = rand2D(floor(i.uv * rand1D(_Time.y * _DisplacementMoveSpeed) * _BlockSize));
                //float displace = rand2D(floor((i.uv + _Time.y * _DisplacementMoveSpeed)* _BlockSize));
                displace = pow(displace,8.0) * pow(displace,3.0);
                
                float r1 = rand1D(_Time.y * _Speed * 7.0);
                float r2 = rand1D(_Time.y * _Speed * 1.0);
                
                float r = tex2D(_MainTex,i.uv).r;
                float g = tex2D(_MainTex,i.uv + float2(displace * 0.05 * r1,0.0)).g;
                float b = tex2D(_MainTex,i.uv - float2(displace * 0.05 * r2,0.0)).b;

                //return float4(displace,displace,displace,1.0);
                return float4(r,g,b,1.0);
            }
            ENDCG
        }
    }
}
