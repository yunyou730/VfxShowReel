using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ttf_glyph
{
    // 通用字段
    public int index;                     // 字体中的字形索引
    public int symbol;                    // UTF-16 符号
    public int npoints;                   // 所有轮廓中的总点数
    public int ncontours;                 // 轮廓数
    
    // 位域标志
    [StructLayout(LayoutKind.Sequential)]
    public struct GlyphFlags
    {
        public uint Value;

        public bool Composite => (Value & 0x00000001) != 0;
        public uint Reserved => (Value & 0xFFFFFFFE);
    }
    public GlyphFlags flags;

    // 水平字形度量
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public float[] xbounds;               // X 坐标边界(min/max)
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public float[] ybounds;               // Y 坐标边界(min/max)
    
    public float advance;                 // 前进宽度
    public float lbearing;                // 左部间距
    public float rbearing;                // 右部间距

    // 字形轮廓
    public IntPtr outline;                // 字形轮廓指针

    // 用户数据指针数组
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] // 假设 TTF_GLYPH_USERDATA = 4
    public IntPtr[] userdata;
}