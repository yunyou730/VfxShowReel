using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    class AyyGPUParticlesRenderPass : ScriptableRenderPass
    {
        private ComputeShader _computeShader = null;
        private Material _particleMaterial = null;
        private int _particleCount = 100;

        public AyyGPUParticlesRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public void SetupParams(Material particleMaterial,ComputeShader computeShader,int particleCount)
        {
            _computeShader = computeShader;
            _particleMaterial = particleMaterial;
            _particleCount = particleCount;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }

            if (_particleMaterial == null)
            {
                return;
            }
            
            CommandBuffer cmd = CommandBufferPool.Get("AyyGPUParticlesRenderPass");
            using (new ProfilingScope(this.profilingSampler))
            {
                //cmd.DrawProcedural(Matrix4x4.identity, _particleMaterial,0,MeshTopology.Points,1,10);
                cmd.DrawProcedural(Matrix4x4.identity,_particleMaterial,0,MeshTopology.Points,1,1);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
    }
}