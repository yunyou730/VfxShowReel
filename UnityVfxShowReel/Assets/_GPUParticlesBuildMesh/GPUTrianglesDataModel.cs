using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ayy
{
    public class GPUTrianglesDataModel : IDisposable
    {
        /*
               [2]
            /       \ 
          [0] ----- [1]
         */
        // @miao @todo
        public Vector3[] _points =
        {
            new Vector3(-1, -1, 0),
            new Vector3(-1, 1, 0),
            new Vector3(0, 1, 0),
        };
        
        public struct TriangleData
        {
            public Vector3 Pos;
            public float Scale;
            public Color Color;
            public float RotByLocalY;
        }
        
        public ComputeBuffer _trianglesBuffer = null;
        //private int _trianglesCount = 0;

        public void InitTrianglesData(int triangleNum)
        {
            //_trianglesCount = triangleNum;
            
            TriangleData[] trianglesData = new TriangleData[triangleNum];
            for (int i = 0;i < triangleNum;i++)
            {
                var triangle = new TriangleData();
                triangle.Pos = Random.insideUnitSphere * 10.0f;
                triangle.Scale = Random.Range(0.5f, 1.5f);
                triangle.Color = Random.ColorHSV();
                trianglesData[i] = triangle;
            }

            // Fill data to compute buffer
            int stride = Marshal.SizeOf<TriangleData>();
            _trianglesBuffer = new ComputeBuffer(trianglesData.Length, stride);
            _trianglesBuffer.SetData(trianglesData);
        }

        public int GetTriangleCount()
        {
            return _trianglesBuffer.count;
        }

        public int GetVerticesCountEachTriangle()
        {
            return _points.Length;
        }

        public void Dispose()
        {
            _trianglesBuffer?.Release();
            _trianglesBuffer?.Dispose();
        }
    }

}

