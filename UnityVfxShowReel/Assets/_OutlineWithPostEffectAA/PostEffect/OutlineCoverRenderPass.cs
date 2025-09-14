using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy.OutlineWithPostEffect
{
    public class OutlineCoverRenderPass : ScriptableRenderPass
    {
        private LayerMask _layerMask;
        
        ProfilingSampler _profilingSampler = new ProfilingSampler(nameof(OutlineCoverRenderPass));
        private static readonly string kCmdBufferName = "OutlineCover_CmdBuffer";
        private readonly ShaderTagId kShaderTagId = new ShaderTagId("Ayy_OutlineCover");
        
        public OutlineCoverRenderPass()
        {
            
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }

            var sortingSettings = new SortingSettings()
            {
                criteria = SortingCriteria.CommonOpaque,
            };
            var drawingSettings = new DrawingSettings(kShaderTagId, sortingSettings)    // draw by specify LightMode tag
            {
                
            };
            var filterSettings = new FilteringSettings(RenderQueueRange.opaque);
            CommandBuffer cmd = CommandBufferPool.Get(kCmdBufferName);
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                cmd.Clear();
                var renderListParams = new RendererListParams(renderingData.cullResults, drawingSettings,filterSettings);
                var renderList = context.CreateRendererList(ref renderListParams);
                cmd.DrawRendererList(renderList);
                context.ExecuteCommandBuffer(cmd);
            }
            CommandBufferPool.Release(cmd);
        }
        
    }
    
}

