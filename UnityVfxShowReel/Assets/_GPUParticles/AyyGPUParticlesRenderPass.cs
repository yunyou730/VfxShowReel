using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    class AyyGPUParticlesRenderPass : ScriptableRenderPass
    {
        private Material _particleMaterial = null;
        
        private ComputeBuffer _particlesBuffer = null;
        private float _particlePointSize = 1.0f;

        public AyyGPUParticlesRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public void SetupParams(Material particleMaterial,ComputeBuffer particlesBuffer,float particleSize)
        {
            _particleMaterial = particleMaterial;
            _particlesBuffer = particlesBuffer;
            _particlePointSize = particleSize;
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
            cmd.Clear();
            using (new ProfilingScope(this.profilingSampler))
            {
                cmd.SetGlobalBuffer(Shader.PropertyToID("Particles"), _particlesBuffer);
                cmd.SetGlobalFloat(Shader.PropertyToID("PointSize"), _particlePointSize);
                //cmd.DrawProcedural(Matrix4x4.identity,_particleMaterial,0,MeshTopology.Points,1,_particlesBuffer.count);
                cmd.DrawProcedural(Matrix4x4.identity,_particleMaterial,0,MeshTopology.Points,1,_particlesBuffer.count);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
    }
}