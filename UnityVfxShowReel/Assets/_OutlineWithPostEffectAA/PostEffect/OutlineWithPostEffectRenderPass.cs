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
        private RTHandle _originFrameBufferRT = null;

        private bool _enableDilate = true;
        
        ProfilingSampler _profilingSampler = new ProfilingSampler(nameof(OutlineWithPostEffectRenderPass));
        
        private static readonly string kCmdBufferName = "OutlineWithPostEffect_CmdBuffer";
        
        private readonly string kTempRTName1 = "OutlineWithPostEffect_RT1";
        private readonly string kTempRTName2 = "OutlineWithPostEffect_RT2";

        private int _blurTimes;

        private bool _enableOutline = true;
        
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
            RenderingUtils.ReAllocateHandleIfNeeded(ref _originFrameBufferRT, desc, filterMode:FilterMode.Bilinear, name: kTempRTName2);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            RTHandles.Release(_tempRT1);
            RTHandles.Release(_tempRT2);
            RTHandles.Release(_originFrameBufferRT);
        }

        public void SetOutlineEnable(bool enable)
        {
            _enableOutline = enable;
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
                if (_enableOutline)
                {
                    _material.SetFloat(Shader.PropertyToID("_ScreenWidth"), Screen.width);
                    _material.SetFloat(Shader.PropertyToID("_ScreenHeight"), Screen.height);
                    _material.SetTexture(Shader.PropertyToID("_OriginTex"), _originFrameBufferRT);
                
                    RTHandle cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

                    cmd.Blit(cameraTarget,_originFrameBufferRT);    // 临时保存 framebuffer 的内容到 tempRT3 
                    cmd.Blit(cameraTarget, _tempRT2, _material, 0); // 对原始画面做 dilate 
                    
                    //cmd.Blit(_tempRT2, cameraTarget);
                    
                    // 在前面的基础上,继续做 blur
                    
                    for (int i = 0; i < _blurTimes; i++)
                    {
                        cmd.Blit(_tempRT2, _tempRT1, _material, 1); // blur h
                        cmd.Blit(_tempRT1, _tempRT2, _material, 2); // blur v
                    }
                    
                    // 把 原始画面经过后处理的结果, 以及 原始画面的拷贝 _originFrameBufferRT 结合在一起,做输出
                    cmd.Blit(_tempRT2, cameraTarget,_material, 3);
                }
                context.ExecuteCommandBuffer(cmd);
            }                
            CommandBufferPool.Release(cmd);
        }
        
    }
    
}

