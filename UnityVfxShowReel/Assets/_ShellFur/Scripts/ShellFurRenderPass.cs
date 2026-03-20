
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    class ShellFurRenderPass : ScriptableRenderPass
    {
        private static string kProfileTag = "ShellFur";
        private static readonly ProfilingSampler s_RenderFeatureSampler = new ProfilingSampler(kProfileTag);

        // shader properties name
        private static int kShellCnt = Shader.PropertyToID("_ShellFurLayerCount");
        private static int kShellFurLength = Shader.PropertyToID("_ShellFurLength");

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            List<ShellFurMono> shellFurList = ShellFurMono.GetInstanceList();
            if (shellFurList == null)
            {
                return;
            }

            CommandBuffer cmdBuf = CommandBufferPool.Get(kProfileTag);
            cmdBuf.Clear();
            using (new ProfilingScope(cmdBuf, s_RenderFeatureSampler))
            {
                foreach (ShellFurMono grassShellFur in shellFurList)    // Render each GrassShellFur Mono
                {
                    if (!grassShellFur.enabled)
                    {
                        continue;
                    }

                    Material mat = grassShellFur.GetMaterial();
                    Mesh mesh = grassShellFur.GetMesh();
                    int shellCount = grassShellFur.GetShellCount();
                    if (mat == null || mesh == null || shellCount < 1)
                    {
                        continue;
                    }

                    cmdBuf.SetGlobalFloat(kShellCnt, (float)shellCount);
                    cmdBuf.SetGlobalFloat(kShellFurLength, grassShellFur.GetFurLength());
                    cmdBuf.DrawMeshInstanced(mesh, 0, mat, 0, grassShellFur.GetMetrics(), shellCount);
                }
            }
            context.ExecuteCommandBuffer(cmdBuf);
            CommandBufferPool.Release(cmdBuf);
        }
    }
    
}
