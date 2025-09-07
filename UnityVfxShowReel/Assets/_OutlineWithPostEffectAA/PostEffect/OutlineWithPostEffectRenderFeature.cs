using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ayy.OutlineWithPostEffect
{
    
    public class OutlineWithPostEffectRenderFeature : ScriptableRendererFeature
    {
        // post effect pass
        public Material _blitMaterial;
        public RenderPassEvent _postEffectEvent = RenderPassEvent.AfterRenderingPostProcessing;
        [Range(0,10)]public int _blurTimes = 0;
        OutlineWithPostEffectRenderPass _outlinePass;
        
        // prepare mask texture pass
        public RenderPassEvent _outlineMaskEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        OutlineMaskRenderPass _outlineMaskPass;
        
        public override void Create()
        {
            _outlinePass = new OutlineWithPostEffectRenderPass(_blitMaterial,_blurTimes);
            _outlinePass.renderPassEvent = _postEffectEvent;
            
            _outlineMaskPass = new OutlineMaskRenderPass();
            _outlineMaskPass.renderPassEvent = _outlineMaskEvent;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            renderer.EnqueuePass(_outlineMaskPass);
            renderer.EnqueuePass(_outlinePass);
        }
    }
}

