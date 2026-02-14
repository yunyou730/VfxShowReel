using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy
{
    [Obsolete("Obsolete")]
    class LiquidGlassRenderPass : ScriptableRenderPass
    {
        private Material _material = null;
        private RenderTargetHandle _tempRT;
        
        private float _radius;
        //private Vector2 _center;
        private float _offset;
        private float _pow;
        private float _edge;
        private float _aaEdge;
        private Color _col;
        
        public LiquidGlassRenderPass(Material material)
        {
            _material = material;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(_tempRT.id,descriptor,FilterMode.Bilinear);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_tempRT.id);
        }

        public void SetParams(float radius,float offset,float pow,float edge,float aaEdge,Color color)
        {
            _radius = radius;
            _offset = offset;
            _pow = pow;
            _edge = edge;
            _aaEdge = aaEdge;
            _col = color;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            if (!Application.isPlaying)
            {
                return;
            }
            
            CommandBuffer cmdBuf = CommandBufferPool.Get(nameof(LiquidGlassRenderPass));
            cmdBuf.Clear();
            using (new ProfilingSample(cmdBuf, nameof(LiquidGlassRenderPass)))
            {
                cmdBuf.SetGlobalFloat(Shader.PropertyToID("_Radius"),_radius);
                cmdBuf.SetGlobalFloat(Shader.PropertyToID("_Offset"),_offset);
                cmdBuf.SetGlobalFloat(Shader.PropertyToID("_PowFactor"),_pow);
                cmdBuf.SetGlobalFloat(Shader.PropertyToID("_BlurEdge"),_edge);
                cmdBuf.SetGlobalFloat(Shader.PropertyToID("_AAEdge"),_aaEdge);
                cmdBuf.SetGlobalColor(Shader.PropertyToID("_Color"),_col);

                RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;
                cmdBuf.Blit(source, _tempRT.Identifier(), _material);
                cmdBuf.Blit(_tempRT.Identifier(),source);
            }
            context.ExecuteCommandBuffer(cmdBuf);
            CommandBufferPool.Release(cmdBuf);
        }
    }
        
}
