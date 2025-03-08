using Mono.Cecil;
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

        private Material _drawTrianglesMat = null;
        private ParticlesBuildMeshMono _buildMeshMono = null;
        private GPUTrianglesDataModel _trianglesDataModel = null;

        private Material _gpuMeshInstanceMat = null;
        
        public BuildMeshRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            _drawTrianglesMat = new Material(Shader.Find("Ayy/GPUTrianglesInstance"));
            _gpuMeshInstanceMat = new Material(Shader.Find("Ayy/GPUMeshInstance"));
            
            _buildMeshMono = GameObject.FindFirstObjectByType<ParticlesBuildMeshMono>();
            if (_buildMeshMono != null)
            {
                _trianglesDataModel = _buildMeshMono._trianglesDataModel;
            }
        }

        public void SetupParams(Material material,Mesh mesh)
        {
            _testMaterial = material;
            _testMesh = mesh;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }

            if (_buildMeshMono == null)
            {
                _buildMeshMono = GameObject.FindFirstObjectByType<ParticlesBuildMeshMono>();
                if (_buildMeshMono != null)
                {
                    _trianglesDataModel = _buildMeshMono._trianglesDataModel;
                }                            
            }
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // if (renderingData.cameraData.cameraType != CameraType.Game)
            // {
            //     return;
            // }

            if (_testMaterial == null)
            {
                return;
            }

            if (_trianglesDataModel == null)
            {
                return;
            }
            
            CommandBuffer cmd = CommandBufferPool.Get("BuildMeshRenderPass");
            cmd.Clear();
            using (new ProfilingScope(this.profilingSampler))
            {
                // draw mesh test
                Matrix4x4 matrix = Matrix4x4.Translate(new Vector3(0, 1, 0));
                cmd.DrawMesh(_testMesh, matrix, _testMaterial);
                
                // gpu instance triangle test
                int verticesCountEachTriangle = _trianglesDataModel.GetVerticesCountEachTriangle();
                int trianglesCount = _trianglesDataModel.GetTriangleCount(); 
                cmd.DrawProcedural(Matrix4x4.identity, _drawTrianglesMat,0,
                                    MeshTopology.Triangles,
                                    verticesCountEachTriangle * trianglesCount, 
                                    trianglesCount);
                
                // gpu instance mesh
                cmd.DrawMeshInstancedProcedural(_testMesh,0,_gpuMeshInstanceMat,0,100);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
    }
}