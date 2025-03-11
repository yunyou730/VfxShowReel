using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    class CustomParticleRenderPass : ScriptableRenderPass
    {
        private Material _particleMaterial = null;
        private CustomParticleSystem _customParticleSysMono = null;
        // private float _particlePointSize = 1.0f;
        // private Mesh _particleMesh = null;

        public CustomParticleRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            _particleMaterial = new Material(Shader.Find("Ayy/MeshEmitParticles"));
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }

            if (_customParticleSysMono == null)
            {
                _customParticleSysMono = GameObject.FindFirstObjectByType<CustomParticleSystem>();
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

            if (_customParticleSysMono == null || _customParticleSysMono.ParticlesBuffer == null)
            {
                return;
            }
            
            
            CommandBuffer cmd = CommandBufferPool.Get("CustomParticleRenderPass");
            cmd.Clear();
            using (new ProfilingScope(this.profilingSampler))
            {
                var particlesBuffer = _customParticleSysMono.ParticlesBuffer;
                cmd.SetGlobalBuffer(Shader.PropertyToID("Particles"), particlesBuffer);
                cmd.DrawProcedural(Matrix4x4.identity,_particleMaterial,0,MeshTopology.Points,1,particlesBuffer.count);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
    }
}