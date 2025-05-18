using System;
using System.Runtime.InteropServices;

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ttf_mesh3d
{
    public int nvert;  // 顶点数组长度
    
    public int nfaces; // 面数组长度
    
    // 顶点坐标数组指针
    public IntPtr vert;
    
    // 面索引数组指针
    public IntPtr faces;
    
    // 法线数组指针（长度为 3*nfaces）
    public IntPtr normals;
    
    // 轮廓指针
    public IntPtr outline;

    // 嵌套结构体：顶点坐标
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public float x;
        public float y;
        public float z;
    }

    // 嵌套结构体：面索引（三角形）
    [StructLayout(LayoutKind.Sequential)]
    public struct Face
    {
        public int v1; // 第一个顶点索引
        public int v2; // 第二个顶点索引
        public int v3; // 第三个顶点索引
    }
}