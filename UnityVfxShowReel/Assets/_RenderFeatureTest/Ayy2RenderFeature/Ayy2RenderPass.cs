using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Ayy2RenderPass : ScriptableRenderPass
{
    private Mesh _mesh = null;
    private Material _material = null;
    
    public Ayy2RenderPass(Mesh mesh,Material material)
    {
        _mesh = mesh;
        _material = material;
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game)
        {
            return;
        }

        if (_mesh == null || _material == null)
        {
            return;
        }

        var cmd = CommandBufferPool.Get("Ayy2");
        cmd.Clear();
        using (new ProfilingScope(profilingSampler))
        {
            cmd.DrawMesh(_mesh, Matrix4x4.identity, _material,0,5);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
        
    }
}
