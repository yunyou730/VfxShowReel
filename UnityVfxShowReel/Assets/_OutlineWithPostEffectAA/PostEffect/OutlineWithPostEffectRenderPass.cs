using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy.OutlineWithPostEffect
{
    public class OutlineWithPostEffectRenderPass : ScriptableRenderPass
    {
        private Material _material = null;

        private RTHandle _tempRT1 = null;
        private RTHandle _tempRT2 = null;

        private bool _enableDilate = true;
        
        ProfilingSampler _profilingSampler = new ProfilingSampler(nameof(OutlineWithPostEffectRenderPass));
        
        private static readonly string kCmdBufferName = "OutlineWithPostEffect_CmdBuffer";
        
        private readonly string kTempRTName1 = "OutlineWithPostEffect_RT1";
        private readonly string kTempRTName2 = "OutlineWithPostEffect_RT2";

        private int _blurTimes;
        
        public OutlineWithPostEffectRenderPass(Material material,int blurTimes)
        {
            _material = material;
            _blurTimes = blurTimes;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.colorFormat = RenderTextureFormat.ARGB32;
            desc.depthStencilFormat = GraphicsFormat.None;
            
            //RenderingUtils.ReAllocateHandleIfNeeded(ref _dilateRT,desc,filterMode:FilterMode.Bilinear,name:kDilateRTName);
            
            RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT1, desc, filterMode:FilterMode.Bilinear,name: kTempRTName1);
            RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT2, desc, filterMode:FilterMode.Bilinear, name: kTempRTName2);
            
            
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            //RTHandles.Release(_dilateRT);
            RTHandles.Release(_tempRT1);
            RTHandles.Release(_tempRT2);
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get(kCmdBufferName);
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                cmd.Clear();
                
                _material.SetFloat(Shader.PropertyToID("_ScreenWidth"), Screen.width);
                _material.SetFloat(Shader.PropertyToID("_ScreenHeight"), Screen.height);
                
                RTHandle cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

                
                cmd.Blit(cameraTarget, _tempRT2, _material, 0); // dilate 
                for (int i = 0; i < _blurTimes; i++)
                {
                    cmd.Blit(_tempRT2, _tempRT1, _material, 1); // blur h
                    cmd.Blit(_tempRT1, _tempRT2, _material, 2); // blur v
                }
                cmd.Blit(_tempRT2, cameraTarget);
                
                //cmd.Blit(cameraTarget, _tempRT1, _material, 0); // dilate
                //cmd.Blit(_tempRT1, _tempRT2, _material, 1); // blur x
                //cmd.Blit(_tempRT2, cameraTarget);   // output to screen
                context.ExecuteCommandBuffer(cmd);
            }                
            CommandBufferPool.Release(cmd);
        }
        
    }
    
}

