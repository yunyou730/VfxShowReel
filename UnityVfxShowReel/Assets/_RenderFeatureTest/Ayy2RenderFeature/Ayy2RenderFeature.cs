using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Ayy2RenderFeature : ScriptableRendererFeature
{
    [SerializeField] public Mesh _mesh;
    [SerializeField] public Material _material;

    private Ayy2RenderPass _pass = null;
    
    public override void Create()
    {
        _pass = new Ayy2RenderPass(_mesh,_material);
    }
    
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        _pass.ConfigureTarget(renderer.cameraColorTargetHandle);
        _pass.ConfigureInput(ScriptableRenderPassInput.Color);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }
}
