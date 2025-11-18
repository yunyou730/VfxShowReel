using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy.glitch
{
    public enum EGlitchType
    {
        None,
        RGBSplitHorizontal,
        RGBSplitVertical,
        RGBSplitHorizontalVertical,
        ImageBlock,
        LineBlock,
        TileJitter,
    }

    public class GlitchRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] EGlitchType _glitchType = EGlitchType.None;
        
        [Header("RGB Split")]
        [SerializeField] private Material _RGBSplitHorizontal = null;
        [SerializeField] private Material _RGBSplitVertical = null;
        [SerializeField] private Material _RGBSplitHorizontalVertical = null;
        
        [Header("Image Block")]
        [SerializeField] private Material _imageBlock = null;
        
        [Header("Line Block")]
        [SerializeField] private Material _lineBlock = null;
        
        private GlitchRenderPass _rgbSplitPassHorizontal = null;
        private GlitchRenderPass _rgbSplitPassVertical = null;
        private GlitchRenderPass _rgbSplitPassHorizontalVertical = null;
        private GlitchRenderPass _imageBlockPass = null;
        private GlitchRenderPass _lineBlockPass = null;
    
        public override void Create()
        {
            _rgbSplitPassHorizontal = new GlitchRenderPass(_RGBSplitHorizontal);
            _rgbSplitPassVertical = new GlitchRenderPass(_RGBSplitVertical);
            _rgbSplitPassHorizontalVertical = new GlitchRenderPass(_RGBSplitHorizontalVertical);
            _imageBlockPass = new GlitchRenderPass(_imageBlock);
            _lineBlockPass = new GlitchRenderPass(_lineBlock);
            
            _rgbSplitPassHorizontal.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            _rgbSplitPassVertical.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            _rgbSplitPassHorizontalVertical.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            _imageBlockPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            _lineBlockPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            ScriptableRenderPass pass = null;
            switch (_glitchType)
            {
                case  EGlitchType.RGBSplitHorizontal:
                    pass = _rgbSplitPassHorizontal;
                    break;
                case  EGlitchType.RGBSplitVertical:
                    pass = _rgbSplitPassVertical;
                    break;
                case EGlitchType.RGBSplitHorizontalVertical:
                    pass = _rgbSplitPassHorizontalVertical;
                    break;
                case EGlitchType.ImageBlock:
                    pass = _imageBlockPass;
                    break;
                case EGlitchType.LineBlock:
                    pass = _lineBlockPass;
                    break;
                default:
                    break;
            }

            if (pass != null)
            {
                renderer.EnqueuePass(pass);    
            }
        }
    }
}
