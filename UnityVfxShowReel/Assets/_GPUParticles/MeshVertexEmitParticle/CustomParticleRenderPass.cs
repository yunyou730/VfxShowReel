using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    class CustomParticleRenderPass : ScriptableRenderPass
    {
        private Material _particlePointsMaterial = null;
        private CustomParticleSystem _customParticleSysMono = null;

        private Material _particleMeshMaterial = null;
        
        public CustomParticleRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            //renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            _particlePointsMaterial = new Material(Shader.Find("Ayy/MeshEmitPointParticles"));
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

            if (_particlePointsMaterial == null)
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
                
                
                if (_customParticleSysMono.ParticleRendererMesh != null
                    && _customParticleSysMono.ParticleRendererMaterial != null)
                {
                    Mesh rendererMesh = _customParticleSysMono.ParticleRendererMesh;
                    Material rendererMaterial = _customParticleSysMono.ParticleRendererMaterial;
                    cmd.DrawMeshInstancedProcedural(rendererMesh,0,rendererMaterial,0,particlesBuffer.count);
                }
                else
                {
                    cmd.DrawProcedural(Matrix4x4.identity,_particlePointsMaterial,0,MeshTopology.Points,1,particlesBuffer.count);                    
                }

                
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
    }
}