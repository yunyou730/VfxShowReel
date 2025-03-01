using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ayy
{
    struct ParticleData
    {
        public Vector3 pos; // float3
        public Color color; // float4
    }

    public class ManualParticleSystemByCS : MonoBehaviour
    {
        public int _particleCount = 1000;
        private ParticleSystem _particleSystem;
        
        private ComputeBuffer _buffer = null;

        public ComputeShader _csParticle = null;
        
        void Start()
        {
            int size = Marshal.SizeOf<ParticleData>();
            _buffer = new ComputeBuffer(_particleCount, size);
            ParticleData[] data = new ParticleData[_particleCount];
            _buffer.SetData(data);
            
            _particleSystem = GetComponent<ParticleSystem>();
            
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Emit();
            }
            UpdateDataBufferWithCS();
        }

        private void Emit()
        {
            _particleSystem.Emit(_particleCount);
        }

        private void UpdateDataBufferWithCS()
        {
            int kernel = _csParticle.FindKernel("CSMain");
            _csParticle.SetBuffer(kernel, Shader.PropertyToID("ParticlesBuffer"), _buffer);
            _csParticle.SetFloat(Shader.PropertyToID("Time"), Time.time);
            _csParticle.Dispatch(kernel, Mathf.CeilToInt(_particleCount/1000f),1, 1);
        }
        
    }
    
}
