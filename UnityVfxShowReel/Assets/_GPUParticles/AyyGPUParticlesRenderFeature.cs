using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy
{
    public class AyyGPUParticlesRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private Material _particleMaterial = null;
        [SerializeField] private Material _trianglesMaterial = null;
        [SerializeField] private int _trianglesCount = 1;
        
        private AyyGPUParticlesRenderPass _gpuParticleRenderPass = null;
        private AyyGPUProceduralTrianglePass _proceduralTrianglePass = null;
        
        public override void Create()
        {
            _gpuParticleRenderPass = new AyyGPUParticlesRenderPass();
            _proceduralTrianglePass = new AyyGPUProceduralTrianglePass(_trianglesCount);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            var partSys = AyyParticleSystem.sInstance; 
            if (partSys != null)
            {
                _gpuParticleRenderPass.SetupParams(_particleMaterial,partSys.GetParticlesBuffer(),partSys.GetParticleSize());                
            }
            _proceduralTrianglePass.SetupParams(_trianglesMaterial);
        }        
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (AyyParticleSystem.sInstance != null)
            {
                renderer.EnqueuePass(_gpuParticleRenderPass);
                renderer.EnqueuePass(_proceduralTrianglePass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _proceduralTrianglePass.Cleanup();   
            }
        }
    }
}