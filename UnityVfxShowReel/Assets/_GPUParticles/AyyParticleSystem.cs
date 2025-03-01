using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace ayy
{
    struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
    }
    
    public class AyyParticleSystem : MonoBehaviour
    {
        [SerializeField] private int _particleCount = 100;
        [SerializeField] private float _particleSize = 2.0f;
        private ComputeBuffer _particlesBuffer = null;
        private Particle[] _particlesData = null;

        public static AyyParticleSystem sInstance = null;
        
        Stopwatch _stopwatch = new Stopwatch();
        
        void Start()
        {
            sInstance = this;
            
            int dataSize = Marshal.SizeOf<Particle>();
            _particlesBuffer = new ComputeBuffer(_particleCount, dataSize);
            _particlesData = new Particle[_particleCount];
            
            _stopwatch.Restart();
            for (int i = 0;i < _particleCount;i++)
            {
                var particle = new Particle();
                particle.position = Random.insideUnitSphere * 5.0f;
                particle.velocity = Vector3.zero;
                _particlesData[i] = particle;
            }
            _particlesBuffer.SetData(_particlesData);
            _stopwatch.Stop();
            Debug.Log("init cost time:" + _stopwatch.ElapsedMilliseconds);
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

        public float GetParticleSize()
        {
            return _particleSize;
        }
    }
}

