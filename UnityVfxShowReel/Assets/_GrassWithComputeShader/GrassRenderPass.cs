using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    class GrassRenderPass : ScriptableRenderPass
    {
        private Material _material = null;
        private Mesh[] _meshWithLODs = null;
        private ComputeShader _grassCS = null;
        private ComputeBuffer _grassBuffer = null;
        private int _grassNum = 0;

        private int _grassGeneratorKernel = 0;

        private static string kCmdBufName = "Ayy_GrassRenderPass";

        public GrassRenderPass(int grassNum,Material mat,Mesh[] meshWithLODs,ComputeShader grassCS,ComputeBuffer grassBuffer)
        {
            _grassNum = grassNum;
            _material = mat;
            _meshWithLODs = meshWithLODs;
            _grassCS = grassCS;
            _grassBuffer = grassBuffer;
            _grassGeneratorKernel = _grassCS.FindKernel("CSMain");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            uint tx, ty, tz;
            _grassCS.GetKernelThreadGroupSizes(_grassGeneratorKernel, out tx, out ty,out tz);
            _grassCS.SetBuffer(_grassGeneratorKernel, Shader.PropertyToID("_GrassBuffer"), _grassBuffer);
            _grassCS.Dispatch(_grassGeneratorKernel,_grassNum / 64,1,1);
            
            GrassRenderData[] data = new GrassRenderData[_grassNum];
            _grassBuffer.GetData(data);
            //Debug.Log(data);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
        
        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // @miao @todo
            CommandBuffer cmd = CommandBufferPool.Get(kCmdBufName);
            cmd.Clear();
            using (new ProfilingScope(this.profilingSampler))
            {
                if (_meshWithLODs.Length > 0)
                {
                    //var particlesBuffer = _customParticleSysMono.ParticlesBuffer;
                    cmd.SetGlobalBuffer(Shader.PropertyToID("_ayy_GrassData"), _grassBuffer);
                    cmd.SetGlobalMatrix(Shader.PropertyToID("_ayy_MatrixIdentity"), Matrix4x4.identity);
                    Mesh rendererMesh = _meshWithLODs[0];
                    Material rendererMaterial = _material;
                    cmd.DrawMeshInstancedProcedural(rendererMesh,0,rendererMaterial,0,_grassBuffer.count);
                    //cmd.DrawMeshInstancedProcedural(rendererMesh,0,rendererMaterial,0,128);
                }
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
