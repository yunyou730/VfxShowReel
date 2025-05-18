using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ayy.ttf
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ttf_file
    {
        public int nchars;                   // 字体字符数
        public int nglyphs;                  // 字形数
        public IntPtr chars;                 // UTF-32 代码数组指针
        public IntPtr char2glyph;            // 字形索引数组指针
        public IntPtr glyphs;                // 字形数组指针
        [MarshalAs(UnmanagedType.LPStr)]
        public string filename;              // 字体文件路径
        
        public uint glyf_csum;               // 'glyf' 表校验和
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public uint[] ubranges;              // Unicode 范围位图

        // head 表结构
        [StructLayout(LayoutKind.Sequential)]
        public struct HeadTable
        {
            public float rev;                // 字体版本
            
            // Mac 样式位域
            [StructLayout(LayoutKind.Sequential, Size = 1)]
            public struct MacStyleFlags
            {
                public byte Value;

                public bool Bold => (Value & 0x01) != 0;
                public bool Italic => (Value & 0x02) != 0;
                public bool Underline => (Value & 0x04) != 0;
                public bool Outline => (Value & 0x08) != 0;
                public bool Shadow => (Value & 0x10) != 0;
                public bool Condensed => (Value & 0x20) != 0;
                public bool Extended => (Value & 0x40) != 0;
            }
            public MacStyleFlags macStyle;
        }
        public HeadTable head;

        // OS/2 表结构
        [StructLayout(LayoutKind.Sequential)]
        public struct OS2Table
        {
            public float xAvgCharWidth;        // 平均字符宽度
            public ushort usWeightClass;       // 字重类
            public ushort usWidthClass;        // 字宽类
            public float yStrikeoutSize;       // 删除线大小
            public float yStrikeoutPos;        // 删除线位置
            public short sFamilyClass;         // 字体家族类
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] panose;              // PANOSE 分类
            
            // 字体选择标志位域
            [StructLayout(LayoutKind.Sequential)]
            public struct FsSelectionFlags
            {
                public ushort Value;

                public bool Italic => (Value & 0x0001) != 0;
                public bool Underscore => (Value & 0x0002) != 0;
                public bool Negative => (Value & 0x0004) != 0;
                public bool Outlined => (Value & 0x0008) != 0;
                public bool Strikeout => (Value & 0x0010) != 0;
                public bool Bold => (Value & 0x0020) != 0;
                public bool Regular => (Value & 0x0040) != 0;
                public bool Utm => (Value & 0x0080) != 0;
                public bool Oblique => (Value & 0x0100) != 0;
            }
            public FsSelectionFlags fsSelection;
            
            public float sTypoAscender;        // 排版上升高度
            public float sTypoDescender;       // 排版下降高度
            public float sTypoLineGap;         // 排版行间距
            public float usWinAscent;          // Windows 上升高度
            public float usWinDescent;         // Windows 下降高度
        }
        public OS2Table os2;

        // name 表结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NameTable
        {
            [MarshalAs(UnmanagedType.LPStr)] public string copyright;    // 版权信息
            [MarshalAs(UnmanagedType.LPStr)] public string family;       // 字体系列名
            [MarshalAs(UnmanagedType.LPStr)] public string subfamily;    // 字体子系列名
            [MarshalAs(UnmanagedType.LPStr)] public string unique_id;    // 唯一标识符
            [MarshalAs(UnmanagedType.LPStr)] public string full_name;    // 完整字体名
            [MarshalAs(UnmanagedType.LPStr)] public string version;      // 版本字符串
            [MarshalAs(UnmanagedType.LPStr)] public string ps_name;      // PostScript 名称
            [MarshalAs(UnmanagedType.LPStr)] public string trademark;    // 商标
            [MarshalAs(UnmanagedType.LPStr)] public string manufacturer; // 制造商名称
            [MarshalAs(UnmanagedType.LPStr)] public string designer;     // 设计师
            [MarshalAs(UnmanagedType.LPStr)] public string description;  // 描述
            [MarshalAs(UnmanagedType.LPStr)] public string url_vendor;   // 供应商 URL
            [MarshalAs(UnmanagedType.LPStr)] public string url_designer; // 设计师 URL
            [MarshalAs(UnmanagedType.LPStr)] public string license_desc; // 许可证描述
            [MarshalAs(UnmanagedType.LPStr)] public string locense_url;  // 许可证 URL
            [MarshalAs(UnmanagedType.LPStr)] public string sample_text;  // 示例文本
        }
        public NameTable names;

        // hhea 表结构
        [StructLayout(LayoutKind.Sequential)]
        public struct HheaTable
        {
            public float ascender;           // 上升高度
            public float descender;          // 下降高度
            public float lineGap;            // 行间距
            public float advanceWidthMax;    // 最大前进宽度
            public float minLSideBearing;    // 最小左部间距
            public float minRSideBearing;    // 最小右部间距
            public float xMaxExtent;         // 最大 X 范围
            public float caretSlope;         // 光标斜率
        }
        public HheaTable hhea;

        // 用户数据指针数组
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] // 假设 TTF_FILE_USERDATA = 4
        public IntPtr[] userdata;
    }
}

