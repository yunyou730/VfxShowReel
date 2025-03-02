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
        public Vector2 position;
        public Vector2 velocity;
    }
    
    public class AyyParticleSystem : MonoBehaviour
    {
        [SerializeField] private int _particleCount = 100;
        [SerializeField] private float _particleSize = 2.0f;
        private ComputeBuffer _particlesBuffer = null;
        private Particle[] _particlesData = null;
        
        [SerializeField] private ComputeShader _csParticleResolver = null;
        private int _csKernelID = 0;

        public static AyyParticleSystem sInstance = null;
        
        Stopwatch _stopwatch = new Stopwatch();

        private float[] _mousePos = new float[2];
        
        void Start()
        {
            sInstance = this;
            InitParticleBuffer();
            _csKernelID = _csParticleResolver.FindKernel("CSMain");
        }

        private void InitParticleBuffer()
        {
            int dataSize = Marshal.SizeOf<Particle>();
            _particlesBuffer = new ComputeBuffer(_particleCount, dataSize);
            _particlesData = new Particle[_particleCount];
            
            _stopwatch.Restart();
            for (int i = 0;i < _particleCount;i++)
            {
                var particle = new Particle();
                particle.position = Random.insideUnitCircle * 10.0f;
                particle.velocity = Vector3.zero;
                _particlesData[i] = particle;
            }
            _particlesBuffer.SetData(_particlesData);
            _stopwatch.Stop();
            Debug.Log("init cost time:" + _stopwatch.ElapsedMilliseconds);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _particlesBuffer.SetData(_particlesData);
            }
            
            _csParticleResolver.SetInt(Shader.PropertyToID("shouldMove"), Input.GetMouseButton(0) ? 1:0);
            RefreshMousePosition();
            _csParticleResolver.SetFloats(Shader.PropertyToID("mousePosition"), _mousePos);
            _csParticleResolver.SetFloat(Shader.PropertyToID("dt"), Time.deltaTime);
            _csParticleResolver.SetBuffer(_csKernelID, Shader.PropertyToID("Particles"), _particlesBuffer);
            _csParticleResolver.Dispatch(_csKernelID,Mathf.CeilToInt((float)_particleCount/1024),1,1);
        }
        
        
        void RefreshMousePosition()
        {
            var mp = Input.mousePosition;
            mp.z = 10.0f;
            var v = Camera.main.ScreenToWorldPoint(mp);
            _mousePos[0] = v.x;
            _mousePos[1] = v.y; 
            //Debug.Log("mouse pos:" + _mousePos[0] + ", " + _mousePos[1] + " mp:" + mp);
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

