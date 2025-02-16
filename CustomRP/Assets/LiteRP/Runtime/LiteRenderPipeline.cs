using System.Collections.Generic;
using LiteRP.FrameData;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public class LiteRenderPipeline : RenderPipeline
    {
        private readonly static ShaderTagId s_ShaderTagId = new ShaderTagId("SRPDefaultUnlit");

        private RenderGraph _renderGraph = null;
        private LiteRenderGraphRecorder _liteRenderGraphRecorder = null;
        private ContextContainer _contextContainer = null;      // hold frame data

        public LiteRenderPipeline()
        {
            InitializeRenderGraph();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CleanupRenderGraph();
        }

        private void InitializeRenderGraph()
        {
            _renderGraph = new RenderGraph("LiteRPRenderGraph");
            _liteRenderGraphRecorder = new LiteRenderGraphRecorder();
            _contextContainer = new ContextContainer();
        }

        private void CleanupRenderGraph()
        {   
            _contextContainer.Dispose();
            _contextContainer = null;

            _liteRenderGraphRecorder = null;
            
            _renderGraph.Cleanup();
            _renderGraph = null;
        }

        private bool PrepareFrameData(ScriptableRenderContext context, Camera camera)
        {
            ScriptableCullingParameters cullingParameters;
            if (!camera.TryGetCullingParameters(out cullingParameters))
            {
                return false;
            }
            
            CullingResults cullingResults = context.Cull(ref cullingParameters);
            
            
            CameraData cameraData = _contextContainer.GetOrCreate<CameraData>();
            cameraData.camera = camera;
            cameraData.cullingResults = cullingResults;
            
            
            return true;
        }

        private void RecordAndExecuteRenderGraph(ScriptableRenderContext context,Camera camera, CommandBuffer cmd)
        {
            RenderGraphParameters parameters = new RenderGraphParameters()
            {
                executionName = camera.name,
                commandBuffer = cmd,
                scriptableRenderContext = context,
                currentFrameIndex = Time.frameCount,
            };
            
            _renderGraph.BeginRecording(parameters);
            {
                // Start Recording
                _liteRenderGraphRecorder.RecordRenderGraph(_renderGraph, _contextContainer);
            }
            _renderGraph.EndRecordingAndExecute();
            
        }


        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            // do nothing
        }
        
        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            BeginContextRendering(context,cameras);
            foreach (var camera in cameras)
            {
                RenderCamera(context, camera);
            }
            _renderGraph.EndFrame();
            EndContextRendering(context,cameras);
        }


        private void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            BeginCameraRendering(context, camera);
            {
                if (!PrepareFrameData(context, camera))
                {
                    return;
                }
                
                context.SetupCameraProperties(camera);
            
                // Prepare command buffer
                CommandBuffer cmd = CommandBufferPool.Get(camera.name);
                
                //RenderByRendererList(camera,context, cmd, ref cullingResults);
                RecordAndExecuteRenderGraph(context, camera, cmd);
                

                // Execute command buffer
                context.ExecuteCommandBuffer(cmd);
                
                // Release command buffer             
                cmd.Clear();
                CommandBufferPool.Release(cmd);
                
                // Context submit                                             
                context.Submit();
            }

            EndCameraRendering(context, camera);
        }
        
        /*
        private void RenderByRendererList(Camera camera,ScriptableRenderContext context,CommandBuffer cmd,ref CullingResults cullingResults)
        {
            
                // Clear Flags & Do Clear 
                bool clearSkybox = camera.clearFlags == CameraClearFlags.Skybox;
                bool clearDepth = camera.clearFlags != CameraClearFlags.Nothing;
                bool clearColor = camera.clearFlags == CameraClearFlags.Color;
            
            
                cmd.ClearRenderTarget(clearDepth, clearColor, CoreUtils.ConvertSRGBToActiveColorSpace(camera.backgroundColor));
                
                // 2. SortSettings,DrawingSettings,FilterSettings
                var sortingSettings = new SortingSettings(camera);
                // Drawing Opaque
                {
                    // prepare renderer list
                    sortingSettings.criteria = SortingCriteria.CommonOpaque;
                    
                    var drawingSettings = new DrawingSettings(s_ShaderTagId, sortingSettings);
                    var filterSettings = new FilteringSettings(RenderQueueRange.opaque);
                    
                    var rendererListParams = new RendererListParams(cullingResults,drawingSettings,filterSettings);
                    var rendererList = context.CreateRendererList(ref rendererListParams);
                    
                    // draw 
                    cmd.DrawRendererList(rendererList);                    
                }
                

                // Draw skybox
                if(clearSkybox)
                {
                    var skyboxRendererList = context.CreateSkyboxRendererList(camera);
                    cmd.DrawRendererList(skyboxRendererList);
                }
                
                // Drawing Transparent
                {
                    // prepare renderer list 
                    sortingSettings.criteria = SortingCriteria.CommonTransparent;
                    var drawingSettings = new DrawingSettings(s_ShaderTagId, sortingSettings);                    
                    var filterSettings = new FilteringSettings(RenderQueueRange.transparent);
                    var rendererListParams = new RendererListParams(cullingResults,drawingSettings,filterSettings);
                    var rendererList = context.CreateRendererList(ref rendererListParams);
                    
                    // draw
                    cmd.DrawRendererList(rendererList);               
                }

        }
        */

    }
}