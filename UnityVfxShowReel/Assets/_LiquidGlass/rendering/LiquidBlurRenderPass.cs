using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    class LiquidBlurRenderPass : ScriptableRenderPass
    {
        private Material _blurMaterial = null;
        
        // ping-pong RT
        private RTHandle _tempRT1;
        private RTHandle _tempRT2;
        
        private const string kCmdBufName = "cmdbuf_" + nameof(LiquidBlurRenderPass);
        private const string kProfilingSamplerName = "profiler_" + nameof(LiquidBlurRenderPass);
        
        private const string kRT1Name = "rt1_" + nameof(LiquidBlurRenderPass);
        private const string kRT2Name = "rt2_" + nameof(LiquidBlurRenderPass);
        private const string kRTBlurName = "rt_blur_" + nameof(LiquidBlurRenderPass);
        
        // Blur Result RenderTexture
        private RTHandle _blurResultRT;
        public static readonly int kBlurResultTextureID = Shader.PropertyToID("_LiquidBlurRenderTexture");

        private float _downSampleScale = 0.5f;
        private int _iterations = 4;
        private int _kernelSize = 5;
        private Vector4 _renderTextureSize = Vector4.one;

        private static class ShaderParams
        {
            public static int KernelSize = Shader.PropertyToID("_KernelSize");
            public static int InputTextureSize = Shader.PropertyToID("_InputTextureSize");
        }
        

        public LiquidBlurRenderPass(Material material)
        {
            _blurMaterial = material;
        }

        public void SetParams(float downSampleScale,int iterations,int kernelSize)
        {
            _downSampleScale = downSampleScale;
            _iterations = iterations;
            _kernelSize = kernelSize;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // 创建 ping-pong RT
            RenderTextureDescriptor downSampleDesc = renderingData.cameraData.cameraTargetDescriptor;
            int width = (int)(downSampleDesc.width * _downSampleScale);
            int height = (int)(downSampleDesc.height * _downSampleScale);
            downSampleDesc.depthBufferBits = 0;
            downSampleDesc.width = width;
            downSampleDesc.height = height;

            RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT1, downSampleDesc, name: kRT1Name);
            RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT2, downSampleDesc, name: kRT2Name);
            
            _renderTextureSize.x = width;       // 记录 PingPong RT size
            _renderTextureSize.y = height;
            _renderTextureSize.z = 0.0f;
            _renderTextureSize.w = 0.0f;
            
            // 创建结果 RT
            RenderingUtils.ReAllocateHandleIfNeeded(ref _blurResultRT, downSampleDesc, name: kRTBlurName); 
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            RTHandles.Release(_tempRT1);
            _tempRT1 = null;
            RTHandles.Release(_tempRT2);
            _tempRT2 = null;            
            RTHandles.Release(_blurResultRT);
            _blurResultRT = null;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            if (!Application.isPlaying)
            {
                return;
            }

            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            
            CommandBuffer cmd = CommandBufferPool.Get(kCmdBufName);
            cmd.Clear();
            using (new ProfilingSample(cmd, kProfilingSamplerName))
            {
                cmd.SetGlobalInt(ShaderParams.KernelSize,_kernelSize);
                cmd.SetGlobalVector(ShaderParams.InputTextureSize,_renderTextureSize);
                cmd.Blit(source, _tempRT1);
                for (int it = 0; it < _iterations; it++)
                {
                    cmd.Blit(_tempRT1, _tempRT2, _blurMaterial, 0);
                    cmd.Blit(_tempRT2, _tempRT1, _blurMaterial, 1);
                }
                cmd.Blit(_tempRT1, _blurResultRT,_blurMaterial,2);      // 绘制在 离屏buffer上
                cmd.SetGlobalTexture(kBlurResultTextureID, _blurResultRT);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
