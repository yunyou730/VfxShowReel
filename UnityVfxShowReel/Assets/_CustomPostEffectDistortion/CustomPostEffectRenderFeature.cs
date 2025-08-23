using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy.CustomPostEffectDistortion
{
    public class CustomRenderPassFeature : ScriptableRendererFeature
    {
        public Shader _shader;
        public RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public DistortionData _distortionData;
        CustomPostEffectRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new CustomPostEffectRenderPass(_shader,_distortionData)
            {
                renderPassEvent = _renderPassEvent,
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview)
            {
                return;
            }
            if (renderingData.cameraData.cameraType == CameraType.SceneView)
            {
                return;
            }
            renderer.EnqueuePass(_renderPass);
        }
    }
}

