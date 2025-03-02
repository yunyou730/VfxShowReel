using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    class AyyGPUProceduralTrianglePass : ScriptableRenderPass
    {
        struct CustomVertexAttribute
        {
            public Vector3 position;
        }
        
        private Material _material = null;
        private ComputeBuffer _buffer = null;
        private CustomVertexAttribute[] _trianglesData = null;
        private int _trianglesCount = 1;
        
        private ProfilingSampler _profilingSampler = new ProfilingSampler(nameof(AyyGPUProceduralTrianglePass));

        public AyyGPUProceduralTrianglePass(int trianglesCount)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

            _trianglesCount = trianglesCount;
            
            _trianglesData = new CustomVertexAttribute[_trianglesCount * 3];
            for (int i = 0;i < _trianglesCount;i++)
            {
                // vertex 1
                _trianglesData[i * 3] = new CustomVertexAttribute();
                //Vector2 pos2d = Random.insideUnitSphere * 5.0f;
                Vector2 pos2d = new Vector2(-1f,-1f) * 5.0f;
                _trianglesData[i * 3].position = new Vector3(pos2d.x,pos2d.y,0.0f);
                
                // vertex 2
                _trianglesData[i * 3 + 1] = new CustomVertexAttribute();
                //pos2d = Random.insideUnitSphere * 5.0f;
                pos2d = new Vector2(1f,-1f) * 5.0f;
                _trianglesData[i * 3 + 1].position = new Vector3(pos2d.x,pos2d.y,0.0f);
                
                // vertex 3
                _trianglesData[i * 3 + 2] = new CustomVertexAttribute();
                //pos2d = Random.insideUnitSphere * 5.0f;
                pos2d = new Vector2(0.0f, 1f) * 5.0f;
                _trianglesData[i * 3 + 2].position = new Vector3(pos2d.x,pos2d.y,0.0f);
            }

            int stride = Marshal.SizeOf<CustomVertexAttribute>() * 3;
            _buffer = new ComputeBuffer(_trianglesCount, stride);
            _buffer.SetData(_trianglesData);
        }
        
        public void Cleanup()
        {
            Debug.Log("AyyGpuProceduralTrianglePass Cleanup");    
            _buffer?.Release();
            _buffer = null;
        }
        
        public void SetupParams(Material material)
        {
            _material = material;
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

            if (_material == null)
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("AyyGPUProceduralTrianglePass");
            cmd.Clear();
            using (new ProfilingScope(_profilingSampler))
            {
                cmd.SetGlobalBuffer(Shader.PropertyToID("CustomBufferData"), _buffer);
                cmd.DrawProcedural(Matrix4x4.identity,_material,0,MeshTopology.Triangles,3,_buffer.count);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
    }
}