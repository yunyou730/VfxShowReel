using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LiteRP
{
    public class LiteRenderPipeline : RenderPipeline
    {
        private readonly static ShaderTagId s_ShaderTagId = new ShaderTagId("SRPDefaultUnlit");
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            // base.Render(context, cameras);
        }
        
        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            BeginContextRendering(context,cameras);
            foreach (var camera in cameras)
            {
                RenderCamera(context, camera);
            }
            EndContextRendering(context,cameras);
        }


        private void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            BeginCameraRendering(context, camera);
            {
                // Culling 
                if (!camera.TryGetCullingParameters(out var cullingParams))
                {
                    return;
                }
                CullingResults cullingResults = context.Cull(ref cullingParams);
                context.SetupCameraProperties(camera);
            
                // Prepare command buffer
                CommandBuffer cmd = CommandBufferPool.Get(camera.name);
            
                // 1. Clear render target
                cmd.ClearRenderTarget(true, true, CoreUtils.ConvertSRGBToActiveColorSpace(camera.backgroundColor));
            
                // 2. SortSettings,DrawingSettings,FilterSettings
                var sortingSettings = new SortingSettings(camera);

                // Drawing Opaque
                {
                    // prepare renderer list
                    sortingSettings.criteria = SortingCriteria.CommonOpaque;
                    
                    var drawingSettings = new DrawingSettings(s_ShaderTagId, sortingSettings);
                    var filterSettings = new FilteringSettings(RenderQueueRange.opaque);
                    
                    var rendererListParams = new RendererListParams(cullingResults,drawingSettings,filterSettings);
                    var rendererList = context.CreateRendererList(ref rendererListParams);
                    
                    // draw 
                    cmd.DrawRendererList(rendererList);                    
                }
                
                // Drawing Transparent
                {
                    // prepare renderer list 
                    sortingSettings.criteria = SortingCriteria.CommonTransparent;
                    var drawingSettings = new DrawingSettings(s_ShaderTagId, sortingSettings);                    
                    var filterSettings = new FilteringSettings(RenderQueueRange.transparent);
                    var rendererListParams = new RendererListParams(cullingResults,drawingSettings,filterSettings);
                    var rendererList = context.CreateRendererList(ref rendererListParams);
                    
                    // draw
                    cmd.DrawRendererList(rendererList);               
                }

                // Execute command buffer
                context.ExecuteCommandBuffer(cmd);
                // Release command buffer             
                cmd.Clear();
                CommandBufferPool.Release(cmd);
                
                // Context submit                                             
                context.Submit();
            }

            EndCameraRendering(context, camera);
        }
        
    }
}