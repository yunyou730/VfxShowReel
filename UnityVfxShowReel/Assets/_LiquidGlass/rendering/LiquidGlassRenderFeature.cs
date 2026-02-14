using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy
{
    public class LiquidGlassRenderFeature : ScriptableRendererFeature
    {
        public Material _liquidGlassMaterial = null;
        public Material _liquidBlurMaterial = null;

        [Range(1,10)]public int _blurIterations = 4;
        [Range(0.1f,1.0f)]public float _blurDownSampleScale = 0.5f;
        [Range(1,5)] public int _blurKernelSize = 5;

        private LiquidGlassRenderPass _liquidGlassPass = null;
        private LiquidBlurRenderPass _blurPass = null;

        public override void Create()
        {
            // blur
            _blurPass = new LiquidBlurRenderPass(_liquidBlurMaterial);
            _blurPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            // uv distortion
            _liquidGlassPass = new LiquidGlassRenderPass(_liquidGlassMaterial);
            _liquidGlassPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _blurPass.SetParams(_blurDownSampleScale,_blurIterations,_blurKernelSize);
            renderer.EnqueuePass(_blurPass);
            renderer.EnqueuePass(_liquidGlassPass);
        }
    }
}
