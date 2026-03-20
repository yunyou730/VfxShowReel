using UnityEngine.Rendering.Universal;

namespace ayy
{
    public class ShellFurRenderFeature : ScriptableRendererFeature
    {
        private ShellFurRenderPass _renderPass = null;
        
        public override void Create()
        {
            _renderPass = new ShellFurRenderPass();
            _renderPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_renderPass);
        }
    }
}
