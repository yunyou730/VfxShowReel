using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class AyySplitScreenRenderPass : ScriptableRenderPass
{
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("AyySplitScreenRenderPass");
    RTHandle m_CameraColorTarget;
    
    int _rtName1 = Shader.PropertyToID("_rtName1");
    int _rtName2 = Shader.PropertyToID("_rtName2");
    
    
    private Material _blitWithColorMat1 = null;
    private Material _blitWithColorMat2 = null;
    private Material _blitWithColorMat3 = null;
    private Material _blitWithColorMat4 = null;
    private Material _splitScreenMat = null;

    public AyySplitScreenRenderPass()
    {
        _blitWithColorMat1 = new Material(Shader.Find("Ayy/BlitWithColor"));
        _blitWithColorMat1.SetColor(Shader.PropertyToID("_Color"), Color.red);
        
        _blitWithColorMat2 = new Material(Shader.Find("Ayy/BlitWithColor"));
        _blitWithColorMat2.SetColor(Shader.PropertyToID("_Color"), Color.green);
        
        _blitWithColorMat3 = new Material(Shader.Find("Ayy/BlitWithColor"));
        _blitWithColorMat3.SetColor(Shader.PropertyToID("_Color"), Color.blue);
        
        _blitWithColorMat4 = new Material(Shader.Find("Ayy/BlitWithColor"));
        _blitWithColorMat4.SetColor(Shader.PropertyToID("_Color"), Color.yellow);
        
        
        _splitScreenMat = new Material(Shader.Find("Ayy/SplitScreen"));
        
        // @miao @todo
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        
    }

    public void SetTarget(RTHandle colorHandle)
    {
        m_CameraColorTarget = colorHandle;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureTarget(m_CameraColorTarget);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        
    }
    

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cameraData = renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game)
            return;

        if (m_CameraColorTarget == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            cmd.GetTemporaryRT(_rtName1, Screen.width, Screen.height, 0);
            cmd.GetTemporaryRT(_rtName2, Screen.width, Screen.height, 0);
            
            cmd.Blit(m_CameraColorTarget,_rtName1,_blitWithColorMat1,0);
            cmd.Blit(m_CameraColorTarget,_rtName2,_blitWithColorMat2,0);
            
            cmd.SetGlobalTexture(Shader.PropertyToID("_Tex1"), _rtName1);
            cmd.SetGlobalTexture(Shader.PropertyToID("_Tex2"), _rtName2);
            cmd.Blit(m_CameraColorTarget,m_CameraColorTarget,_splitScreenMat,0);
            
            
            cmd.ReleaseTemporaryRT(_rtName1);
            cmd.ReleaseTemporaryRT(_rtName2);
            
        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }
}