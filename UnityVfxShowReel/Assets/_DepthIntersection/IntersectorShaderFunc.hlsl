#ifndef DEPTH_INTERSECTOR_SHADER_INCLUDED
#define DEPTH_INTERSECTOR_SHADER_INCLUDED


float3 GetViewPosWithScreenUVAndDepth(float4 screenPos,float linear01Depth)
{
    float2 screenUV = screenPos.xy;

    float2 ndcPos = screenUV * 2 - 1;
    float3 clipPos = float3(ndcPos.x,ndcPos.y,1.0) * _ProjectionParams.z;
    
    float3 viewPos = mul(unity_CameraInvProjection,clipPos.xyzz).xyz * linear01Depth;
    return viewPos;
}

void MyFunction_float(float4 screenPos,float linear01Depth,float3 fragViewPos,float threshold,out float Out)
{
    float3 depthViewPos = GetViewPosWithScreenUVAndDepth(screenPos,linear01Depth);
    float diff = distance(depthViewPos,fragViewPos);
    
    Out = 0.33;
    if(diff < threshold)
    {
        Out = 0.89;
    }
}

#endif