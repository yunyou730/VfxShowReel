#ifndef DEPTH_INTERSECTION_SHADER_INCLUDED
#define DEPTH_INTERSECTION_SHADER_INCLUDED


float3 GetWorldPosWithScreenUVAndDepth(float4 screenPos,float linear01Depth)
{
    float2 screenUV = screenPos.xy;

    float2 ndcPos = screenUV * 2 - 1;
    float3 clipPos = float3(ndcPos.x,ndcPos.y,1.0) * _ProjectionParams.z;
    
    float3 viewPos = mul(unity_CameraInvProjection,clipPos.xyzz).xyz * linear01Depth;
    float3 worldPos = mul(UNITY_MATRIX_I_V,float4(viewPos,1)).xyz;

    return worldPos;
}

void MyFunction_float(float4 screenPos,float linear01Depth,float3 fragWorldPos,out float3 Out)
{
    float3 depthWorldPos = GetWorldPosWithScreenUVAndDepth(screenPos,linear01Depth);
    float3 depthModelPos = mul(UNITY_MATRIX_I_M,float4(depthWorldPos,1)).xyz;
    //float dis = distance(depthWorldPos,depthModelPos);

    Out = float3(1,1,1);
    if(abs(depthModelPos.x) <= 0.5 && abs(depthModelPos.y) <= 0.5 && abs(depthModelPos.z) <= 0.5)
    {
        Out = float3(1,0,0); 
    }
    else
    {
        discard;
    }
}

#endif //DEPTH_INTERSECTION_SHADER_INCLUDED