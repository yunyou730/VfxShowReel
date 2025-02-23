using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy.rendering
{
    public class LUTRenderFeature : ScriptableRendererFeature
    {
        private LUTRenderPass _lutPass = null;
        [SerializeField] private Shader _lutShader = null;
        private Material _lutMaterial = null;
        
        public override void Create()
        {
            _lutMaterial = new Material(_lutShader);
            _lutPass = new LUTRenderPass(_lutMaterial);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            _lutPass.SetTarget(renderer.cameraColorTargetHandle);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            renderer.EnqueuePass(_lutPass);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CoreUtils.Destroy(_lutMaterial);
        }
    }
}

