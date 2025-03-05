using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy
{
    public class BuildMeshRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private Material _testMaterial = null;
        [SerializeField] private Mesh _testMesh = null;
        
        private BuildMeshRenderPass _buildMeshRenderPass = null;
        
        public override void Create()
        {
            _buildMeshRenderPass = new BuildMeshRenderPass();
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                _buildMeshRenderPass.SetupParams(_testMaterial, _testMesh);                
            }
        }        
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(_buildMeshRenderPass);                
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                //_buildMeshRenderPass.Cleanup();   
            }
        }
    }
}