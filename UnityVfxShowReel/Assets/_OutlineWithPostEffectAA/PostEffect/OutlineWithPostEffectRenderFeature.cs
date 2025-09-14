using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ayy.OutlineWithPostEffect
{
    public class OutlineWithPostEffectRenderFeature : ScriptableRendererFeature
    {
        // outline pass
        public bool _enableOutlinePass = true;
        public Material _blitMaterial;
        public RenderPassEvent _postEffectEvent = RenderPassEvent.AfterRenderingPostProcessing;
        [Range(0,10)]public int _blurTimes = 0;
        OutlineWithPostEffectRenderPass _outlinePass;
        
        // prepare mask texture pass
        public bool _enableMaskPass = true;
        public RenderPassEvent _outlineMaskEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        OutlineMaskRenderPass _outlineMaskPass;
        
        // draw object cover outline
        public bool _enableCoverPass = true;
        public RenderPassEvent _outlineCoverEvent = RenderPassEvent.BeforeRenderingPostProcessing;    
        public OutlineCoverRenderPass _outlineCoverPass;
        
        public override void Create()
        {
            _outlinePass = new OutlineWithPostEffectRenderPass(_blitMaterial,_blurTimes);
            _outlinePass.renderPassEvent = _postEffectEvent;
            
            _outlineMaskPass = new OutlineMaskRenderPass();
            _outlineMaskPass.renderPassEvent = _outlineMaskEvent;

            _outlineCoverPass = new OutlineCoverRenderPass();
            _outlineCoverPass.renderPassEvent = _outlineCoverEvent;
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

            if (_enableMaskPass)
            {
                renderer.EnqueuePass(_outlineMaskPass);                
            }
            //if (_enableOutlinePass)
            //{
                _outlinePass.SetOutlineEnable(_enableOutlinePass);
                renderer.EnqueuePass(_outlinePass);                
            //}
            if (_enableCoverPass)
            {
                renderer.EnqueuePass(_outlineCoverPass);                
            }
        }
    }
}

