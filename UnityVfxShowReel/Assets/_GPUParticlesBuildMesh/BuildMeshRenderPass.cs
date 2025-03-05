using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    class BuildMeshRenderPass : ScriptableRenderPass
    {
        private Material _testMaterial = null;
        private Mesh _testMesh = null;
        
        //private ComputeBuffer _particlesBuffer = null;
        //private float _particlePointSize = 1.0f;

        public BuildMeshRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public void SetupParams(Material material,Mesh mesh)
        {
            _testMaterial = material;
            _testMesh = mesh;
            //_particlesBuffer = particlesBuffer;
            //_particlePointSize = particleSize;
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

            if (_testMaterial == null)
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("BuildMeshRenderPass");
            cmd.Clear();
            using (new ProfilingScope(this.profilingSampler))
            {
                //cmd.SetGlobalBuffer(Shader.PropertyToID("Particles"), _particlesBuffer);
                //cmd.SetGlobalFloat(Shader.PropertyToID("PointSize"), _particlePointSize);
                //cmd.DrawProcedural(Matrix4x4.identity,_particleMaterial,0,MeshTopology.Points,1,_particlesBuffer.count);
                //cmd.DrawProcedural(Matrix4x4.identity,_particleMaterial,0,MeshTopology.Points,1,_particlesBuffer.count);
                cmd.DrawMesh(_testMesh, Matrix4x4.identity, _testMaterial);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
    }
}