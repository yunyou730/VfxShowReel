using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = Unity.Mathematics.Random;

namespace ayy
{
    public class CustomParticleSystem : MonoBehaviour
    {
        public struct Particle
        {
            public Vector3 Position;
            public Vector3 Velocity;
            public float Active;
            public float ElapsedTime;
            public float LifeTime;
            public Vector3 Color1;
            public Vector3 Color2;
        }

        public struct MeshVertex
        {
            public Vector3 Position;
            public Vector3 Normal;
        }

        [SerializeField] public int _particlePoolSize = 100;
        [SerializeField] public Mesh _particleShapeMesh = null;
        [SerializeField,Range(0.1f,3.0f)] public float _particleShapeScale = 1.0f;
        [SerializeField] public float _particleInitialSpeed = 10.0f;
        [SerializeField,Range(-5,5)] public float _particleAcc = 0.0f;
        [SerializeField] public float _particleLifeTime = 3.0f;
        [SerializeField] public ComputeShader _particleMovementCS = null;
        [SerializeField,Range(1,10)] public int _skipVertexCount = 1;

        [SerializeField] public Mesh _particleRendererMesh = null;
        [SerializeField] Material _particleRendererMaterial = null;
        [SerializeField,Range(0,1)] private float _particleRendererMeshScale = 1.0f;
        
        [SerializeField] private Transform _shapeCursor = null;
        [SerializeField,Range(5f,20f)] private float _mouseZ = 10.0f;
        [SerializeField,Range(0,10)] private float _mouseScrollSpeed = 3.0f;
        
        [SerializeField,Range(1000,10000)] private float _keyboardMoveSpeed = 2000.0f;
        [SerializeField] private bool _bEnableMouseCtrlPos = false;

        private ComputeBuffer _particlesBuffer = null;
        private Particle[] _particlesData = null;
        private int _nextToUseIndex = 0;

        private ComputeBuffer _meshVerticesBuffer = null;

        public ComputeBuffer ParticlesBuffer { get { return _particlesBuffer; } }
        
        
        // 作用在每一个 粒子上的 mesh & material 
        public Mesh ParticleRendererMesh { get { return _particleRendererMesh; } }
        public Material ParticleRendererMaterial { get { return _particleRendererMaterial; } }

        private int _kernelMoveToMeshVert = 0;
        private int _kernelUpdateParticles = 0;

        private Stopwatch _stopwatch = null;

        private Vector3[] _meshVertices = null;
        private Vector3[] _meshNormals = null;

        void Start()
        {
            _stopwatch = new Stopwatch();
            HoldMeshVertexData();
            InitParticleBuffer();
            InitMeshVerticesBuffer();
            InitComputeShader();
        }

        void Update()
        {
            UpdateControlByMouse();                
            UpdateControlByKeyboard();
            UpdateParticles(Time.deltaTime);
        }

        private void UpdateControlByMouse()
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mouseZ);
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            if (!_bEnableMouseCtrlPos && _shapeCursor != null)
            {
                mouseWorldPos = _shapeCursor.position;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.L))
            //if(Input.GetMouseButton(0))
            {
                EmitParticlesByMeshGPU(ref _meshVertices,ref _meshNormals, mouseWorldPos);
            }
            
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            float scrollDelta = mouseScroll * _mouseScrollSpeed;// * Time.deltaTime;
            _mouseZ += scrollDelta;
            
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                EmitParticlesByMeshCPU(ref _meshVertices,ref _meshNormals, mouseWorldPos);
            }
            
            if (_shapeCursor != null && _bEnableMouseCtrlPos)
            {
                _shapeCursor.position = mouseWorldPos;
            }
        }

        private void UpdateControlByKeyboard()
        {
            if (_shapeCursor == null)
            {
                return;
            }
            
            Vector3 moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                moveDir += Vector3.up;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDir += Vector3.down;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDir += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDir += Vector3.right;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                moveDir += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.E))
            {
                moveDir += Vector3.back;
            }

            if (moveDir.sqrMagnitude > Mathf.Epsilon)
            {
                moveDir.Normalize();
                _shapeCursor.position += moveDir * _keyboardMoveSpeed * Time.deltaTime;
            }
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                Vector3 worldPos = _shapeCursor.position;
                EmitParticlesByMeshGPU(ref _meshVertices,ref _meshNormals, worldPos);
            }
        }

        private void OnDestroy()
        {
            _particlesBuffer?.Release();
            _particlesBuffer = null;

            _meshVerticesBuffer?.Release();
            _meshVerticesBuffer = null;
        }

        private void HoldMeshVertexData()
        {
            _stopwatch.Restart();

            // hold all 
            //_meshVertices = _particleShapeMesh.vertices;
            //_meshNormals = _particleShapeMesh.normals;
            _meshVertices = new Vector3[_particleShapeMesh.vertexCount / _skipVertexCount];
            _meshNormals = new Vector3[_particleShapeMesh.vertexCount / _skipVertexCount];
            for (int i = 0,subIndex = 0;i < _particleShapeMesh.vertexCount;i += _skipVertexCount,subIndex++)
            {
                if (i >= _meshVertices.Length)
                {
                    break;
                }
                _meshVertices[subIndex] = _particleShapeMesh.vertices[i] * _particleShapeScale;
                _meshNormals[subIndex] = _particleShapeMesh.normals[i];
            }

            _stopwatch.Stop();
            Debug.Log($"HoldMeshVertexData cost {_stopwatch.ElapsedMilliseconds} ms.");
        }

        private void InitParticleBuffer()
        {
            _stopwatch.Restart();
            _particlesData = new Particle[_particlePoolSize];
            for (int i = 0; i < _particlePoolSize; i++)
            {
                var particle = new Particle();
                ResetParticle(ref particle);
                _particlesData[i] = particle;
            }

            _stopwatch.Stop();
            Debug.Log($"ayy- init particle buffer,data cost {_stopwatch.ElapsedMilliseconds} ms");

            _stopwatch.Restart();
            _particlesBuffer = new ComputeBuffer(_particlePoolSize, Marshal.SizeOf<Particle>());
            _particlesBuffer.SetData(_particlesData);
            _stopwatch.Stop();
            Debug.Log($"ayy- init particle buffer,buffer cost {_stopwatch.ElapsedMilliseconds} ms");
        }

        private void InitMeshVerticesBuffer()
        {
            List<MeshVertex> meshVertexData = new List<MeshVertex>();
            //for (int i = 0; i < .vertices.Length; i++)
            for(int i = 0;i < _meshVertices.Length;i++)
            {
                var meshVertex = new MeshVertex();
                meshVertex.Position = _meshVertices[i];
                meshVertex.Normal = _meshNormals[i];
                meshVertexData.Add(meshVertex);
            }

            _meshVerticesBuffer = new ComputeBuffer(_particlePoolSize, Marshal.SizeOf<MeshVertex>());
            _meshVerticesBuffer.SetData(meshVertexData);
        }

        private void InitComputeShader()
        {
            _kernelMoveToMeshVert = _particleMovementCS.FindKernel("CSMoveToMeshVertex");
            _kernelUpdateParticles = _particleMovementCS.FindKernel("CSUpdate");
            _particleMovementCS.SetBuffer(_kernelMoveToMeshVert, Shader.PropertyToID("_ParticleBuffer"),
                _particlesBuffer);
            _particleMovementCS.SetBuffer(_kernelMoveToMeshVert, Shader.PropertyToID("_MeshVertexBuffer"),
                _meshVerticesBuffer);
            _particleMovementCS.SetBuffer(_kernelUpdateParticles, Shader.PropertyToID("_ParticleBuffer"),
                _particlesBuffer);
        }

        private void EmitParticlesByMeshGPU(ref Vector3[] vertices,ref Vector3[] normals, Vector3 worldPos)
        {
            // 从粒子池发射第一波, 计算新激活粒子的下标范围 from,to
            int from = _nextToUseIndex;
            int to = _nextToUseIndex + vertices.Length;
            if (to >= _particlePoolSize)
            {
                to = _particlePoolSize - 1;
            }

            _nextToUseIndex = (to + 1) % _particlePoolSize;

            // 如果粒子池已经用到底, 则从头开始复用, 计算新激活粒子的下标范围 from2,to2
            int from2 = -1;
            int to2 = -1;
            int emittedCnt = (to - from + 1);
            int wantEmitCnt = vertices.Length;
            int notEnoughCnt = wantEmitCnt - emittedCnt;
            if (notEnoughCnt > 0 && notEnoughCnt < wantEmitCnt)
            {
                from2 = 0;
                to2 = notEnoughCnt - 1;
                _nextToUseIndex = (to2 + 1) % _particlePoolSize;
            }

            // emit, 只传新激活的 下标范围 [from,to] 和 [from2,to2] 即可
            DoComputeParticlesEmitAndMove(worldPos, from, to, from2, to2);
        }

        private void DoComputeParticlesEmitAndMove(Vector3 worldPos, int fromParticleIndex, int toParticleIndex,
            int from2, int to2)
        {
            Color fromColor = UnityEngine.Random.ColorHSV();
            Color toColor = UnityEngine.Random.ColorHSV();
            
            _particleMovementCS.SetInt(Shader.PropertyToID("_StartIndex"), fromParticleIndex);
            _particleMovementCS.SetInt(Shader.PropertyToID("_ToIndex"), toParticleIndex);
            _particleMovementCS.SetInt(Shader.PropertyToID("_From2"), from2);
            _particleMovementCS.SetInt(Shader.PropertyToID("_To2"), to2);
            _particleMovementCS.SetFloats(Shader.PropertyToID("_TargetWorldPosition"), new float[3]{worldPos.x,worldPos.y,worldPos.z });
            _particleMovementCS.SetFloat(Shader.PropertyToID("_ParticleSpeed"), _particleInitialSpeed);
            _particleMovementCS.SetFloat(Shader.PropertyToID("_ParticleAcc"), _particleAcc);
            _particleMovementCS.SetFloat(Shader.PropertyToID("_ParticleLifeTime"), _particleLifeTime);
            _particleMovementCS.SetFloats(Shader.PropertyToID("_emitStartColor"), new float[3]{fromColor.r,fromColor.g,fromColor.b});
            _particleMovementCS.SetFloats(Shader.PropertyToID("_emitEndColor"), new float[3]{toColor.r, toColor.g, toColor.b});            
            _particleMovementCS.Dispatch(_kernelMoveToMeshVert, Mathf.CeilToInt((float)_particlePoolSize / 64), 1, 1);
        }

        private void EmitParticlesByMeshCPU(ref Vector3[] vertices,ref Vector3[] normals, Vector3 worldPos)
        {
            _stopwatch.Restart();

            int count = vertices.Length;

            int emmitCount = 0;
            int fromSubIndex = _nextToUseIndex;
            for (int i = 0; i < count; i++)
            {
                if (_nextToUseIndex >= _particlesData.Length)
                {
                    break;
                }

                Vector3 pos = vertices[i];
                Vector3 normal = normals[i];

                ref Particle particle = ref _particlesData[_nextToUseIndex];
                EmitOneParticle(ref particle, pos + worldPos, normal * _particleInitialSpeed, _particleLifeTime);
                emmitCount++;

                // iterate
                _nextToUseIndex++;
                if (_nextToUseIndex >= _particlesData.Length)
                {
                    break;
                }
            }

            _stopwatch.Stop();
            Debug.Log($"ayy- cpu emit {emmitCount} particle data cost:{_stopwatch.ElapsedMilliseconds}");



            if (emmitCount > 0)
            {
                _stopwatch.Restart();
                _particlesBuffer.SetData(_particlesData, fromSubIndex, fromSubIndex, emmitCount);
                //_particlesBuffer.SetData(_particlesData);
                _stopwatch.Stop();
                Debug.Log($"ayy- cpu emit {emmitCount} particle buffer cost:{_stopwatch.ElapsedMilliseconds}");

            }
        }

        private void ResetParticle(ref Particle particle)
        {
            //particle.Position = Vector3.zero;
            particle.Position = UnityEngine.Random.insideUnitSphere * 10.0f;
            particle.Velocity = Vector3.zero;
            particle.Active = 0.0f;
            particle.ElapsedTime = 0.0f;
            particle.LifeTime = 0.0f;
        }

        private void EmitOneParticle(ref Particle particle, Vector3 pos, Vector3 velocity, float lifeTime)
        {
            particle.Position = pos;
            particle.Velocity = velocity;
            particle.Active = 1.0f;
            particle.ElapsedTime = 0.0f;
            particle.LifeTime = lifeTime;
        }

        private void UpdateParticles(float deltaTime)
        {
            _particleRendererMaterial.SetFloat(Shader.PropertyToID("_Scale"),_particleRendererMeshScale);
            _particleMovementCS.SetFloats(Shader.PropertyToID("_DeltaTime"), deltaTime);
            _particleMovementCS.Dispatch(_kernelUpdateParticles, Mathf.CeilToInt((float)_particlePoolSize / 64), 1, 1);
        }

        public void EmitParticlesManually()
        {
            if (_shapeCursor != null)
            {
                EmitParticlesByMeshGPU(ref _meshVertices, ref _meshNormals, _shapeCursor.position);
            }
        }
    }
}
