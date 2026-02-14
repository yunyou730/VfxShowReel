using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy
{
    [Obsolete("Obsolete")]
    class LiquidGlassRenderPass : ScriptableRenderPass
    {
        private Material _material = null;
        private RenderTargetHandle _tempRT;
        
        public LiquidGlassRenderPass(Material material)
        {
            _material = material;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(_tempRT.id,descriptor,FilterMode.Bilinear);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_tempRT.id);
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            if (!Application.isPlaying)
            {
                return;
            }
            
            CommandBuffer cmdBuf = CommandBufferPool.Get(nameof(LiquidGlassRenderPass));
            cmdBuf.Clear();
            using (new ProfilingSample(cmdBuf, nameof(LiquidGlassRenderPass)))
            {
                RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;
                cmdBuf.Blit(source, _tempRT.Identifier(), _material);
                cmdBuf.Blit(_tempRT.Identifier(),source);
            }
            context.ExecuteCommandBuffer(cmdBuf);
            CommandBufferPool.Release(cmdBuf);
        }
    }
        
}
