using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace ayy
{
    public class ParticlesBuildMeshMono : MonoBehaviour
    {
        public Mesh _mesh1 = null;
        public Mesh _mesh2 = null;
        public int _particlesCount = 100;
        public ComputeShader _csParticleResolver = null;
        
        private ComputeBuffer _particlesBuffer = null;
        private ComputeBuffer _mesh1Buffer = null;
        private ComputeBuffer _mesh2Buffer = null;
        
        public GPUTrianglesDataModel _trianglesDataModel = null;
        
        struct MeshVertex
        {
            public Vector3 position;
        }

        struct ParticleVertex
        {
            public Vector3 position;
        }
        
        void Start()
        {
            _mesh1Buffer = InitMeshDataBuffer(_mesh1);
            _mesh2Buffer = InitMeshDataBuffer(_mesh2);
            _particlesBuffer = InitParticlesBuffer(_particlesCount);
            
            _trianglesDataModel = new GPUTrianglesDataModel();
            _trianglesDataModel.InitTrianglesData(10);
        }

        private ComputeBuffer InitMeshDataBuffer(Mesh mesh)
        {
            // data
            List<MeshVertex> data = new List<MeshVertex>();
            int vertexCount = mesh.vertexCount;
            data = new List<MeshVertex>(vertexCount);
            for(int i = 0;i < vertexCount;i++)
            {
                var meshVert = new MeshVertex();
                meshVert.position = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z);
                data.Add(meshVert);
            }
            
            // buffer
            int size = Marshal.SizeOf<MeshVertex>();
            ComputeBuffer buffer = new ComputeBuffer(vertexCount, size);
            buffer.SetData(data);
            return buffer;
        }

        private ComputeBuffer InitParticlesBuffer(int particleCount)
        {
            // data
            List<ParticleVertex> data = new List<ParticleVertex>(particleCount);
            for (int i = 0;i < particleCount;i++)
            {
                var particleVertex = new ParticleVertex();
                particleVertex.position = Random.insideUnitSphere * 5.0f;
                data.Add(particleVertex);
            }
            // buffer
            ComputeBuffer buffer = new ComputeBuffer(particleCount, Marshal.SizeOf<ParticleVertex>());
            return buffer;
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                BuildMesh1();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                BuildMesh2();
            }
        }

        private void OnDestroy()
        {
            _particlesBuffer.Release();
            _mesh1Buffer.Release();
            _mesh2Buffer.Release();
            
            _trianglesDataModel.Dispose();
            _trianglesDataModel = null;
        }

        private void BuildMesh1()
        {
            
        }

        private void BuildMesh2()
        {
            
        }
    }
}

