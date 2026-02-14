using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    public class LiquidGlassRenderFeature : ScriptableRendererFeature
    {
        public Material _liquidGlassMaterial = null;
        public Material _liquidBlurMaterial = null;
        
        [Header("Blur")]
        [Range(1,10)]public int _blurIterations = 4;
        [Range(0.1f,1.0f)]public float _blurDownSampleScale = 0.5f;
        [Range(1,5)] public int _blurKernelSize = 5;

        [Header("Distortion")] 
        [Range(0, 1)] public float _radius = 0.5f;
        [Range(0, 0.3f)] public float _offset = 0.15f;
        [Range(1.0f, 6.0f)] public float _pow = 2.5f;
        public Color _col = Color.white;
        [Range(0, 0.3f)] public float _edge = 0.05f;
        [Range(0, 0.1f)] public float _aaEdge = 0.01f;


        [Header("Debug")] 
        public bool _enableDebugBlur = false;
        public bool _enableDebugDistortion = false;

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
            if (_enableDebugBlur)
            {
                _liquidGlassMaterial.EnableKeyword("_ENABLE_DEBUG_BLUR_WEIGHT");
            }
            else
            {
                _liquidGlassMaterial.DisableKeyword("_ENABLE_DEBUG_BLUR_WEIGHT");
            }

            if (_enableDebugDistortion)
            {
                _liquidGlassMaterial.EnableKeyword("_ENABLE_DEBUG_DISTORTION_WEIGHT");
            }
            else
            {
                _liquidGlassMaterial.DisableKeyword("_ENABLE_DEBUG_DISTORTION_WEIGHT");
            }
            
            _blurPass.SetParams(_blurDownSampleScale,_blurIterations,_blurKernelSize);
            renderer.EnqueuePass(_blurPass);
            _liquidGlassPass.SetParams(_radius,_offset,_pow,_edge,_aaEdge,_col);
            renderer.EnqueuePass(_liquidGlassPass);
        }
    }
}
