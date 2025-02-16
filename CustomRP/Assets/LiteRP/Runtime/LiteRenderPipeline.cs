using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LiteRP
{
    public class LiteRenderPipeline : RenderPipeline
    {
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
            
            // Culling 
            ScriptableCullingParameters cullingParams;
            if (!camera.TryGetCullingParameters(out cullingParams))
            {
                return;
            }
            CullingResults cullingResults = context.Cull(ref cullingParams);
            
            // Prepare command buffer
            CommandBuffer cmd = CommandBufferPool.Get(camera.name);
            
            /*
             *  1. Clear render target
             *  2. SortSettings,DrawingSettings,FilterSettings
             *  3. Create render list
             *  4. Draw render list
             */
            // Execute command buffer
            context.ExecuteCommandBuffer(cmd);
            // Release command buffer             
            cmd.Clear();
            CommandBufferPool.Release(cmd);
            // Context submit                                             
            context.Submit();
            
            EndCameraRendering(context, camera);
        }
        
    }
}