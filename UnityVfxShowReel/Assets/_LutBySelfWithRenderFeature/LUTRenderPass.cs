using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy.rendering
{
    public class LUTRenderPass : ScriptableRenderPass
    {
        private int _tempRTID = Shader.PropertyToID("_TempRT");
        private RTHandle _cameraColorTargetHandle = null;
        private Material _blitMaterial = null;
        
        public LUTRenderPass(Material material)
        {
            renderPassEvent = RenderPassEvent.AfterRendering;
            _blitMaterial = material;
        }

        public void SetTarget(RTHandle cameraColorTargetHandle)
        {
            _cameraColorTargetHandle = cameraColorTargetHandle;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(_tempRTID, descriptor,FilterMode.Bilinear);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_tempRTID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_blitMaterial == null)
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get(nameof(LUTRenderPass));
            cmd.Clear();
            using (new ProfilingScope(profilingSampler))
            {
                cmd.Blit(_cameraColorTargetHandle,_tempRTID,_blitMaterial);
                cmd.Blit(_tempRTID,_cameraColorTargetHandle);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
    
}
