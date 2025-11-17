using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy.glitch
{
    [Obsolete("Obsolete")]
    class GlitchRenderPass : ScriptableRenderPass
    {
        private Material _material = null;
        private RenderTargetHandle _tempRT;

        public GlitchRenderPass(Material material)
        {
            _material = material;
        }

        [Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
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
            CommandBuffer cmdBuf = CommandBufferPool.Get(nameof(GlitchRenderPass));
            cmdBuf.Clear();
            using (new ProfilingSample(cmdBuf, nameof(GlitchRenderPass)))
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
