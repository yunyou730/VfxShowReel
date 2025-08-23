using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy.CustomPostEffectDistortion
{
    class CustomPostEffectRenderPass : ScriptableRenderPass
    {
        private RTHandle _tempRT1 = null;
        private RTHandle _tempRT2 = null;
        private ProfilingSampler _profilingSampler = new ProfilingSampler(nameof(CustomPostEffectRenderPass));
        private Material _material = null;
        private DistortionData _distortionData = null;
        
        private readonly string kCmdBufName = "ayy_custom_post_effect_cmdbuf";
        private readonly string kTempRTName1 = "ayy_custom_post_effect_rt_1";
        private readonly string kTempRTName2 = "ayy_custom_post_effect_rt_2";

        public CustomPostEffectRenderPass(Material material,DistortionData distortionData)
        {
            _material = material;
            _distortionData = distortionData;
        }

        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthStencilFormat = GraphicsFormat.None;
            
            RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT1, desc, filterMode:FilterMode.Bilinear,name: kTempRTName1);
            RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT2, desc, filterMode:FilterMode.Bilinear, name: kTempRTName2);
        }

        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            RTHandles.Release(_tempRT1);
            RTHandles.Release(_tempRT2);
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

            VolumeStack stack = VolumeManager.instance.stack;
            CustomPostEffectVolume volume = stack.GetComponent<CustomPostEffectVolume>();
            if (!volume.IsActive())
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get(kCmdBufName);
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                RTHandle cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
                
                // controlled by ScriptableObject data
                _material.SetFloat(Shader.PropertyToID("_CenterX"),_distortionData.CenterX);
                _material.SetFloat(Shader.PropertyToID("_CenterY"),_distortionData.CenterY);
                _material.SetFloat(Shader.PropertyToID("_ZoomFactor"),_distortionData.ZoomFactor);
                _material.SetFloat(Shader.PropertyToID("_LowerThreshold"),_distortionData.LowerThreshold);
                _material.SetFloat(Shader.PropertyToID("_IncThreshold"),_distortionData.IncThreshold);
                _material.SetFloat(Shader.PropertyToID("_DecThreshold"),_distortionData.DecThreshold);

                // controlled by volume
                _material.SetFloat(Shader.PropertyToID("_Mode"),(float)volume.Mode.value);
                _material.SetFloat(Shader.PropertyToID("_DebugDistortionStrength"),volume.debugDistortionStrength.value ? 1.0f : 0.0f);
                
                _material.SetFloat(Shader.PropertyToID("_WaveAmplitude"),volume.WaveAmplitude.value);
                _material.SetFloat(Shader.PropertyToID("_WaveFreq"),volume.WaveFreq.value);

                _material.SetFloat(Shader.PropertyToID("_ZoomerInner"),volume.ZoomerInner.value);
                _material.SetFloat(Shader.PropertyToID("_ZoomerOuter"),volume.ZoomerOuter.value);
                _material.SetFloat(Shader.PropertyToID("_ZoomerZoomFactor"),volume.ZoomerZoomFactor.value);
                
                // Grids
                _material.SetFloat(Shader.PropertyToID("_CellsNum"),volume.cellsNum.value);
                _material.SetFloat(Shader.PropertyToID("_GridBorder"),volume.gridBorderSize.value);
                _material.SetColor(Shader.PropertyToID("_LineColor"),volume.lineColor.value);
                _material.SetColor(Shader.PropertyToID("_BgColor1"),volume.bgColor1.value);
                _material.SetColor(Shader.PropertyToID("_BgColor2"),volume.bgColor2.value);
                
                cmd.Clear();
                cmd.Blit(cameraTarget,_tempRT1,_material,1);     // grid
                cmd.Blit(_tempRT1,_tempRT2,_material,0);       // distortion
                cmd.Blit(_tempRT2,cameraTarget);       // distortion
                context.ExecuteCommandBuffer(cmd);
            }
            CommandBufferPool.Release(cmd);
        }

    }
}

