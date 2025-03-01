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
        [SerializeField,Range(0,1000000)] private int _particleCount = 100;
        [SerializeField] private Material _particleMaterial = null;
        [SerializeField] private ComputeShader _computeShader = null;
        
        AyyGPUParticlesRenderPass m_ScriptablePass;
        
        public override void Create()
        {
            m_ScriptablePass = new AyyGPUParticlesRenderPass();
            m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            m_ScriptablePass.SetupParams(_particleMaterial,_computeShader,_particleCount);
        }        
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}