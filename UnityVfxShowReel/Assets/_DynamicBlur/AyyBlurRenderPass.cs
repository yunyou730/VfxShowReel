using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy.rendering
{

    class AyyBlurRenderPass : ScriptableRenderPass
    {
        private Material _blitMaterial = null;


        private RTHandle _tempRT = null;

        private RTHandle _sourceRT = null;
        private RTHandle _targetRT = null;
        
        ProfilingSampler _profilingSampler = new ProfilingSampler(nameof(AyyBlurRenderPass));


        public AyyBlurRenderPass()
        {
            _blitMaterial = new Material(Shader.Find("Ayy/BlitShader"));
        }

        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _sourceRT = renderingData.cameraData.renderer.cameraColorTargetHandle;
            _targetRT = renderingData.cameraData.renderer.cameraColorTargetHandle;;
            
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            //_tempRT = RTHandles.Alloc(descriptor);
            //desc.msaaSamples = 1;
            desc.depthStencilFormat = GraphicsFormat.None;
            RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT, desc, name: "_ayy_blit_temp_rt");
        }

        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }

            VolumeStack volumeStack = VolumeManager.instance.stack;

            var blurVolume = volumeStack.GetComponent<AyyBlurVolumeComp>();
            if (blurVolume.IsActive())
            {
                CommandBuffer cmd = CommandBufferPool.Get("AyyBlur");
                using (new ProfilingScope(cmd, _profilingSampler))
                {
                    cmd.Clear();
                    
                    //Blitter.BlitCameraTexture(cmd,_sourceRT, _tempRT, _blitMaterial,0);
                    //Blitter.BlitCameraTexture(cmd,_tempRT, _targetRT,_blitMaterial,0);
                    cmd.Blit(_sourceRT, _tempRT, _blitMaterial,0);
                    cmd.Blit(_tempRT, _sourceRT);
                    context.ExecuteCommandBuffer(cmd);
                }                
                CommandBufferPool.Release(cmd);
            }
            
        }

        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            RTHandles.Release(_tempRT);
            _tempRT = null;
        }
    }
    
}
