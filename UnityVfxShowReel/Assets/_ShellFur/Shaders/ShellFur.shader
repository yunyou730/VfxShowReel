Shader "ayy/ShellFur"
{
    Properties
    {
        [NoScaleOffset] _MainTex("MainTex", 2D) = "white" {}
        [NoScaleOffset] _FurTex("FurTex", 2D) = "white" {}

        [Header(Texture Tiling)]
        _MainTexTiling("Main Tex Tiling",Range(0,10)) = 1.0
        _FurTexTiling("Fur Tex Tiling",Range(0,100)) = 1.0
        
        // BlinnPhong Factors
        [Header(BlinnPhong)]
        _AmbientFactor("Ambient Factor",Range(0,1)) = 1.0
        _DiffuseFactor("Diffuse Factor",Range(0,1)) = 1.0
        _SpecFactor("Spec Factor",Range(0,1)) = 1.0
        
        // 高光颜色
        _SpecColor("Specular Color",Color) = (1,1,1,1)
        _SpecPow("Specular Pow", Range(0.01, 256.0)) = 8.0

        // 采样 FurTex 噪声值来影响 alpha之前,对  alpha值做一个垫底
        [Header(Fur Alpha)]
        _FurBaseAlpha("FurBaseAlpha",Range(0.0,1.0)) = 0.0
        _FurDensity("FurDensity Alpha Desc",Range(0,2)) = 0.11

        // 模拟AO.接近 base层 衰减强， 接近最外层 shell衰减弱
        [Header(AO)]
        _FurShading("Sim AO",Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Cull Back
            ZTest LEqual

            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                sampler2D _MainTex;
                float4 _MainTex_ST;
                
                sampler2D _FurTex;
                float4 _FurTex_ST;

                float _MainTexTiling;
                float _FurTexTiling;

                float _FurShading;      // fur shell index 对 baseColor 的影响
                float _FurBaseAlpha;    // FurTex 的 noise 值, 对 alpha 值影响之前, alpha的基础值
                float _FurDensity;

                // BlinnPhong 各项参数
                float _AmbientFactor;
                float _DiffuseFactor;
                float _SpecFactor;

                float4 _SpecColor;   // 高光颜色
                float _SpecPow;      // 高光次幂
            CBUFFER_END

            float _ShellFurLayerCount;  //  shell 一共有多少层
            float _ShellFurLength;     // shell 最长, 能有多长

            ENDHLSL

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #pragma shader_feature_local _ENABLE_ALPHA_BY_SHELL_INDEX

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;

                uint instanceId     : SV_InstanceID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 uv          : TEXCOORD0;
                float shellIndex   : TEXCOORD2;
                float4 positionWS  : TEXCOORD3;
                float3 normalWS    : TEXCOORD4;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                // 获取 instance index
                UNITY_SETUP_INSTANCE_ID(input); 
                float instancePct = (float)input.instanceId / _ShellFurLayerCount;
                float offset = instancePct * _ShellFurLength;

                // 沿法线外扩
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float3 posOS = input.positionOS.xyz + normalWS * offset;
                //float3 posOS = input.positionOS.xyz;      // 临时测试, 不做外扩
                
                output.positionHCS = TransformObjectToHClip(posOS);
                output.uv.xy = TRANSFORM_TEX(input.uv, _MainTex).xy;
                output.uv.zw = TRANSFORM_TEX(input.uv, _FurTex).xy;
                // output.alpha = alpha;

                output.shellIndex = (float)input.instanceId;
                output.positionWS = float4(TransformObjectToWorld(posOS),1.0);
                output.normalWS = normalWS;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                Light mainLight = GetMainLight();
                
                half4 ambientColor = tex2D(_MainTex, input.uv.xy * _MainTexTiling);
                ambientColor.rgb *= mainLight.color;
                
                // base层: 0,  shell顶层:1;  中间 [0,1]过度
                float shellPct = input.shellIndex / _ShellFurLayerCount;

                // shell 根部,颜色衰减
                ambientColor.rgb -= pow(1 - shellPct,3.0) * _FurShading;

                // 采样 noise texture 的 r值, 扰动 alpha . 调整 _FurTex 的 Tiling 改变 fur密度
                float furTexNoise = tex2D(_FurTex,input.uv.zw * _FurTexTiling).r;
                //float alpha = furTexNoise + _FurBaseAlpha;
                float alpha = clamp(furTexNoise - shellPct * shellPct * _FurDensity,0,1);   // 让顶部对 alpha值为0的面积更大,从而形成底部粗,顶部尖的效果
                
                //alpha = lerp(alpha,1.0,step(shellPct,0.5)); // 最内层 alpha 设置为1.0
                

                // BlinnPhong
                float3 N = normalize(input.normalWS);   // 法线向量
                float3 V = normalize(GetCameraPositionWS() - input.positionWS.xyz); // 从像素,指向相机 
                float3 L = normalize(mainLight.direction);  // 从像素, 指向光源
                float3 H = normalize(L + V);        // L,V 半程向量
                
                // diffuse
                float NdotL = saturate(dot(N,L));
                half3 diffuseColor = ambientColor * NdotL;
                
                // specular
                float spec = pow(saturate(dot(N,H)),_SpecPow);
                half3 specularColor = _SpecColor.rgb * spec;
                
                // BlinnPhong result
                half3 ret = ambientColor.rgb * _AmbientFactor
                            + diffuseColor.rgb * _DiffuseFactor
                            + specularColor.rgb * _SpecFactor;
                
                return half4(ret,alpha);
            }
            ENDHLSL
        }
    }
}   