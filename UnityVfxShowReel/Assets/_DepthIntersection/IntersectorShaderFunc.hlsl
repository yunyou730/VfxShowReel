#ifndef DEPTH_INTERSECTOR_SHADER_INCLUDED
#define DEPTH_INTERSECTOR_SHADER_INCLUDED

//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
// #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariables.hlsl"
// #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
// #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

float3 GetViewPosWithScreenUVAndDepth(float4 screenPos,float linear01Depth)
{
    float2 screenUV = screenPos.xy;

    float2 ndcPos = screenUV * 2 - 1;
    float3 clipPos = float3(ndcPos.x,ndcPos.y,1.0) * _ProjectionParams.z;
    
    float3 viewPos = mul(unity_CameraInvProjection,clipPos.xyzz).xyz * linear01Depth;
    return viewPos;
}

float3 GetWorldPosWithScreenUVAndDepth2(float4 screenPos,float linear01Depth)
{
    float2 screenUV = screenPos.xy;

    float2 ndcPos = screenUV * 2 - 1;
    float3 clipPos = float3(ndcPos.x,ndcPos.y,1.0) * _ProjectionParams.z;
    
    float3 viewPos = mul(unity_CameraInvProjection,clipPos.xyzz).xyz * linear01Depth;
    float3 worldPos = mul(UNITY_MATRIX_I_V,float4(viewPos,1)).xyz;

    return worldPos;
}


void MyFunction_float(float4 screenPos,float linear01Depth,float3 fragViewPos,float3 fragWorldPosParam,float threshold,out float Out)
{
    float3 depthViewPos = GetViewPosWithScreenUVAndDepth(screenPos,linear01Depth);
    float diff = distance(depthViewPos,fragViewPos);
    
    
    float3 depthWorldPos = GetWorldPosWithScreenUVAndDepth2(screenPos,linear01Depth);
    float3 fragWorldPos = mul(UNITY_MATRIX_I_V,float4(fragViewPos,1.0)).xyz;
    diff = distance(depthWorldPos,fragWorldPos);

    diff = distance(depthWorldPos,fragWorldPosParam);
    
    Out = 0.33;
    if(diff < threshold)
    {
        Out = 0.89;
    }
}

#endif