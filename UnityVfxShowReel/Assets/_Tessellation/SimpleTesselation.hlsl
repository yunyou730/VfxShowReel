#ifndef SIMPLE_TESSELLATION_INCLUDED
#define SIMPLE_TESSELLATION_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

CBUFFER_START(UnityPerMaterial)
// Tessellation factors for Hull PatchConstantFunction 
float4 _TessEdgeFactor;
float _TessInsideFactor;
CBUFFER_END

struct TessellationControlPoint
{
    float3 positionWS : INTERNALTESSPOS;
    float3 normalWS : NORMAL;
    float2 UV : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// The hull function runs once per vertex. You can use it to modify vertex data based on values in the entire triangle
[domain("tri")] // Signal we are inputting triangles
[outputcontrolpoints(3)] // triangles have 3 points
[outputtopology("triangle_cw")] // signal we are outputting triangles, clock wise
[patchconstantfunc("PatchConstantFunction")] // Register the patch constant function
[partitioning("integer")] // Select a partitioning mode: integer, fractional_odd, fractional_even, pow2
TessellationControlPoint Hull(
    InputPatch<TessellationControlPoint,3> patch, // Input triangle
    uint id : SV_OutputControlPointID)  // Vertex index on the triangle
{
    return patch[id];
}

struct TessellationFactors
{
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};
            
// The patch sontant funciton runs once per triangle , or "patch"
// It runs in parallel to the hull function
TessellationFactors PatchConstantFunction(
    InputPatch<TessellationControlPoint,3> patch)
{
    UNITY_SETUP_INSTANCE_ID(patch[0]);  // setup instancing

    // Calculate tessellation factors
    TessellationFactors f;
    f.edge[0] = _TessEdgeFactor.x;  // 1.0 by default
    f.edge[1] = _TessEdgeFactor.y;  // 1.0 by default
    f.edge[2] = _TessEdgeFactor.z;  // 1.0 by default
    f.inside = _TessInsideFactor;   // 1.0 by default
    return f;
}


// Domain 
struct Interpolators
{
    float2 UV : TEXCOORD0;
    float3 normalWS : TEXCOORD3;
    float3 positionWS : TEXCOORD4;
    float4 positionCS : SV_POSITION;
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

#define BARYCENTRIC_INTERPOLATE(fieldName) \
        patch[0].fieldName * barycentricCoordinates.x + \
        patch[1].fieldName * barycentricCoordinates.y + \
        patch[2].fieldName * barycentricCoordinates.z
        

[domain("tri")] // singal we are inputting triangles
Interpolators Domain(
    TessellationFactors factors, // The output of the patch constant function
    OutputPatch<TessellationControlPoint,3> patch, // the input triangle, or "patch",
    float3 barycentricCoordinates: SV_DomainLocation // the barycentric coordinates of the vertex on the triangle
)
{
    Interpolators output;
    
    UNITY_SETUP_INSTANCE_ID(patch[0]);
    UNITY_TRANSFER_INSTANCE_ID(patch[0],output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    float3 positionWS = BARYCENTRIC_INTERPOLATE(positionWS);
    float3 normalWS = BARYCENTRIC_INTERPOLATE(normalWS);
    float2 UV = BARYCENTRIC_INTERPOLATE(UV);
    
    output.positionCS = TransformWorldToHClip(positionWS);
    output.normalWS = normalWS;
    output.positionWS = positionWS;
    output.UV = UV;
    
    return output;
}

#endif