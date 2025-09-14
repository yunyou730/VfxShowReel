using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy.OutlineWithPostEffect
{
    public class OutlineMaskRenderPass : ScriptableRenderPass
    {
        private LayerMask _layerMask;
        
        private RTHandle _outlineMaskRT = null;
        
        ProfilingSampler _profilingSampler = new ProfilingSampler(nameof(OutlineMaskRenderPass));
        
        private static readonly string kCmdBufferName = "OutlineMask_CmdBuffer";
        private static readonly int kOutlineMask = Shader.PropertyToID("_AyyOutlineMask");
        private readonly string kRTName = "OutlineMaskRT";
        private readonly ShaderTagId kShaderTagId = new ShaderTagId("Ayy_OutlineMask");
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthStencilFormat = GraphicsFormat.None;
            desc.colorFormat = RenderTextureFormat.R8;
            RenderingUtils.ReAllocateHandleIfNeeded(
                ref _outlineMaskRT,
                desc,
                filterMode:FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                1,0,kRTName
                );
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            RTHandles.Release(_outlineMaskRT);
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
            
            // 不修改 depth,stencil buffer
            //var renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            CommandBuffer cmd = CommandBufferPool.Get(kCmdBufferName);
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                cmd.Clear();
                cmd.SetRenderTarget(_outlineMaskRT);
                cmd.ClearRenderTarget(false, true, Color.black);
                var renderListParams = new RendererListParams(renderingData.cullResults, drawingSettings,filterSettings);
                var renderList = context.CreateRendererList(ref renderListParams);
                cmd.DrawRendererList(renderList);
                cmd.SetGlobalTexture(kOutlineMask,_outlineMaskRT);
                context.ExecuteCommandBuffer(cmd);
            }
            CommandBufferPool.Release(cmd);
        }
        
    }
    
}

