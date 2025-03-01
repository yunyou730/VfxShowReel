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
        AyyGPUParticlesRenderPass m_ScriptablePass;
        
        public override void Create()
        {
            m_ScriptablePass = new AyyGPUParticlesRenderPass();
            m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (AyyParticleSystem.sInstance != null)
            {
                m_ScriptablePass.SetupParams(_particleMaterial,AyyParticleSystem.sInstance.GetParticlesBuffer());                
            }
        }        
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (AyyParticleSystem.sInstance != null)
            {
                renderer.EnqueuePass(m_ScriptablePass);                
            }
        }
    }
}