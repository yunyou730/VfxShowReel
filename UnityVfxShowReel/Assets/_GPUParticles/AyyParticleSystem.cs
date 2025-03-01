using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ayy
{
    struct Particle
    {
        public Vector2 position;
        public Vector2 velocity;
    }
    
    public class AyyParticleSystem : MonoBehaviour
    {
        [SerializeField] private int _particleCount = 100;
        private ComputeBuffer _particlesBuffer = null;
        private Particle[] _particlesData = null;

        public static AyyParticleSystem sInstance = null;
        
        void Start()
        {
            sInstance = this;
            
            int dataSize = Marshal.SizeOf<Particle>();
            _particlesBuffer = new ComputeBuffer(_particleCount, dataSize);
            _particlesData = new Particle[_particleCount];
            for (int i = 0;i < _particleCount;i++)
            {
                var particle = new Particle();
                particle.position = Random.insideUnitCircle * 5.0f;
                particle.velocity = Vector2.zero;
                _particlesData[i] = particle;
            }
            _particlesBuffer.SetData(_particlesData);
        }

        void Update()
        {
            
        }

        void OnDestroy()
        {
            sInstance = null;
            
            _particlesBuffer.Release();
            _particlesBuffer = null;
        }

        public ComputeBuffer GetParticlesBuffer()
        {
            return _particlesBuffer;
        }
    }
}

