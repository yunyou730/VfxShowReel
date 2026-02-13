using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy
{
    public class LiquidGlassRenderFeature : ScriptableRendererFeature
    {
        public Material _liquidGlassMaterial = null;
        private LiquidGlassRenderPass _pass = null;

        public override void Create()
        {
            _pass = new LiquidGlassRenderPass(_liquidGlassMaterial);
            _pass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_pass);
        }
    }
}
