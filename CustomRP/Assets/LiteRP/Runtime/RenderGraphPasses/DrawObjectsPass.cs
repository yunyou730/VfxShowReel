using LiteRP.FrameData;
using PlasticGui.Configuration.CloudEdition;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public class DrawObjectsPass
    {
        
    }

    public partial class LiteRenderGraphRecorder
    {
        private static readonly ProfilingSampler s_DrawObjectsProfilingSampler = new ProfilingSampler("Draw Objects");
        private static readonly ShaderTagId s_shaderTagId = new ShaderTagId("SRPDefaultUnlit");
        
        internal class DrawObjectsPassData
        {
            internal RendererListHandle opaqueRendererListHandle;
            internal RendererListHandle transparentRendererListHandle;
            internal TextureHandle backbufferHandle;
        }


        private void AddDrawObjectsPass(RenderGraph renderGraph,ContextContainer frameData)
        {
            CameraData cameraData = frameData.Get<CameraData>();
            
            
            using (var builder = renderGraph.AddRasterRenderPass<DrawObjectsPassData>(
                       "Draw Objects Pass",
                       out var passData,
                       s_DrawObjectsProfilingSampler))
            {
                // Opaque renderer list
                RendererListDesc opaqueRendererDesc = new RendererListDesc(s_shaderTagId,cameraData.cullingResults,cameraData.camera);
                opaqueRendererDesc.sortingCriteria = SortingCriteria.CommonOpaque;
                opaqueRendererDesc.renderQueueRange = RenderQueueRange.opaque;
                passData.opaqueRendererListHandle = renderGraph.CreateRendererList(opaqueRendererDesc);
                builder.UseRendererList(passData.opaqueRendererListHandle);
                
                
                // Transparent renderer list
                RendererListDesc transparentRendererDesc = new RendererListDesc(s_shaderTagId,cameraData.cullingResults,cameraData.camera);                
                transparentRendererDesc.sortingCriteria = SortingCriteria.CommonTransparent;
                transparentRendererDesc.renderQueueRange = RenderQueueRange.transparent;
                passData.transparentRendererListHandle = renderGraph.CreateRendererList(transparentRendererDesc);
                builder.UseRendererList(passData.transparentRendererListHandle);
                
                // Skybox renderer list
                // @miao @todo
                
                
                // Specify render target
                passData.backbufferHandle = renderGraph.ImportBackbuffer(BuiltinRenderTextureType.CurrentActive);
                builder.SetRenderAttachment(passData.backbufferHandle, 0,AccessFlags.Write);
                
                // 设置渲染全局状态
                builder.AllowPassCulling(false);
                
                
                builder.SetRenderFunc((DrawObjectsPassData passData,RasterGraphContext context) =>
                {
                    // 调用 渲染 指令 
                    context.cmd.DrawRendererList(passData.opaqueRendererListHandle);
                    context.cmd.DrawRendererList(passData.transparentRendererListHandle);                    
                });
            }

        }
    }
}