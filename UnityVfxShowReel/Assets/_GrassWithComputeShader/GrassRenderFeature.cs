using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace ayy
{
    public struct GrassRenderData
    {
        public Vector3 pos;
    }
    public class GrassRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private ComputeShader _computeShader = null;
        [SerializeField] private Material _grassMaterial = null;
        [SerializeField] private Mesh[] _meshWithLODs = null;
        [SerializeField] private RenderPassEvent _grassRenderEvent =  RenderPassEvent.AfterRenderingOpaques;
        [SerializeField] private int _grassNum = 128;       // @miao @test

        private ComputeBuffer _grassBuffer = null;
        private GrassRenderPass _grassRenderPass = null;



        public override void Create()
        {
            // @miao @Todo
            // Create ComputeBuffer
            _grassBuffer = new ComputeBuffer(_grassNum, Marshal.SizeOf<GrassRenderData>());
            
            // Create RenderPass
            _grassRenderPass = new GrassRenderPass(_grassNum,_grassMaterial, _meshWithLODs,_computeShader,_grassBuffer);
            _grassRenderPass.renderPassEvent = _grassRenderEvent;
        }

        protected override void Dispose(bool disposing)
        {
            if (_grassBuffer != null)
            {
                _grassBuffer.Release();
                _grassBuffer = null;
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_grassRenderPass != null && Application.isPlaying)
            {
                renderer.EnqueuePass(_grassRenderPass);
            }
        }
    }
}
