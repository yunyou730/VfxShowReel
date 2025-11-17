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
        [SerializeField] private Material _RGBSplitHorizontal = null;
        [SerializeField] private Material _RGBSplitVertical = null;
        [SerializeField] private Material _RGBSplitHorizontalVertical = null;
        
        private RGBSplit _rgbSplitPassHorizontal = null;
        private RGBSplit _rgbSplitPassVertical = null;
        private RGBSplit _rgbSplitPassHorizontalVertical = null;
    
        public override void Create()
        {
            _rgbSplitPassHorizontal = new RGBSplit(_RGBSplitHorizontal);
            _rgbSplitPassVertical = new RGBSplit(_RGBSplitVertical);
            _rgbSplitPassHorizontalVertical = new RGBSplit(_RGBSplitHorizontalVertical);
            
            _rgbSplitPassHorizontal.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            _rgbSplitPassVertical.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            _rgbSplitPassHorizontalVertical.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
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
