using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy.rendering
{
    public class AyyPostEffectRenderFeature : ScriptableRendererFeature
    {
        private AyyBlurRenderPass _blurPass = null;

        /// <inheritdoc/>
        public override void Create()
        {
            _blurPass = new AyyBlurRenderPass();
            _blurPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_blurPass);
        }
    }
    
}
