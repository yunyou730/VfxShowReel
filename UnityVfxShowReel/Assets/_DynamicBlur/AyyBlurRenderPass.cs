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
        
        private RTHandle _tempRT1 = null;
        private RTHandle _tempRT2 = null;
        
        private RTHandle _sourceRT = null;
        private RTHandle _targetRT = null;
        
        ProfilingSampler _profilingSampler = new ProfilingSampler(nameof(AyyBlurRenderPass));

        private Vector4 _screenSize;
        
        public AyyBlurRenderPass()
        {
            _blitMaterial = new Material(Shader.Find("Ayy/GaussianBlur"));
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
            
            VolumeStack volumeStack = VolumeManager.instance.stack;
            var blurVolume = volumeStack.GetComponent<AyyBlurVolumeComp>();
            if (blurVolume.IsActive())
            {
                float downSampleScale = blurVolume.DownSampleScale.value;
                
                var desc = renderingData.cameraData.cameraTargetDescriptor;
                desc.depthStencilFormat = GraphicsFormat.None;
                desc.width = (int)(desc.width * downSampleScale);
                desc.height = (int)(desc.height * downSampleScale);
            
                RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT1, desc, name: "ayy_gaussianblur_temp_rt");
                _tempRT1.rt.wrapMode = TextureWrapMode.Clamp;
                
                RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT2, desc, name: "ayy_gaussianblur_temp_rt");
                _tempRT2.rt.wrapMode = TextureWrapMode.Clamp;                
            
                _screenSize.x = Screen.width;
                _screenSize.y = Screen.height;
                _screenSize.z = 0.0f;
                _screenSize.w = 0.0f;
            }

            
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

                    cmd.Blit(_sourceRT, _tempRT1);
                    
                    
                    int kernelSize = Mathf.FloorToInt(blurVolume.KernelSize.value);
                    int iterations = Mathf.FloorToInt(blurVolume.Iterations.value);
                    for (int iteration = 0; iteration < iterations; iteration++)
                    {
                        // horizontal blur
                        cmd.SetGlobalInt(Shader.PropertyToID("_kernelSize"),kernelSize);
                        cmd.SetGlobalVector(Shader.PropertyToID("_screenSize"),_screenSize);
                        //cmd.SetGlobalTexture(Shader.PropertyToID("_BlitTexture"),_tempRT1);
                        cmd.Blit(_tempRT1, _tempRT2, _blitMaterial,0);
                        
                        // vertical blur
                        cmd.SetGlobalInt(Shader.PropertyToID("_kernelSize"),kernelSize);
                        cmd.SetGlobalVector(Shader.PropertyToID("_screenSize"),_screenSize);
                        //cmd.SetGlobalTexture(Shader.PropertyToID("_BlitTexture"),_tempRT2);
                        cmd.Blit(_tempRT2, _tempRT1,_blitMaterial,1);
                    }

                    cmd.Blit(_tempRT1, _targetRT);
                    
                    context.ExecuteCommandBuffer(cmd);
                }                
                CommandBufferPool.Release(cmd);
            }
            
        }

        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            RTHandles.Release(_tempRT1);
            _tempRT1 = null;
            
            RTHandles.Release(_tempRT2);
            _tempRT2 = null;            
        }
    }
    
}
