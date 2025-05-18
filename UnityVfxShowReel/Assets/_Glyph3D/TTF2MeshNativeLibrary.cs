using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ayy.ttf
{
    public class TTF2MeshNativeLibrary
{
    private const string DLL_NAME = "libttf2meshdylib";
    
    private const int TTF_GLYPH_USERDATA = 4;
    private const int TTF_FILE_USERDATA = 4;
    
        
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ttf_load_from_file(
        [MarshalAs(UnmanagedType.LPStr)] string filename,
        out IntPtr output,
        [MarshalAs(UnmanagedType.I1)] bool headers_only
    );

    
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern void ttf_free(IntPtr ttf);
    
    
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern int ttf_glyph2mesh3d(
        IntPtr glyph,                // ttf_glyph_t*
        out IntPtr output,           // ttf_mesh3d_t**
        byte quality,                // uint8_t quality
        int features,                // int features
        float depth                  // float depth
    );

    
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ttf_free_mesh3d(IntPtr mesh);
    
    // wrapper
    public static int LoadTTFFile(string filename, out IntPtr ttfHandle, bool headersOnly = false)
    {
        if (string.IsNullOrEmpty(filename))
        {
            ttfHandle = IntPtr.Zero;
            return 0;
        }
        int result = ttf_load_from_file(filename, out ttfHandle, headersOnly);
        return result;
    }

    // wrapper
    public static void FreeTTFFile(IntPtr ttfHandle)
    {
        if (ttfHandle != IntPtr.Zero)
        {
            ttf_free(ttfHandle);
            ttfHandle = IntPtr.Zero;
        }
    }
    
    // wrapper
    public static ttf_glyph GetGlyphAtIndex(IntPtr glyphsPtr,int index)
    {
        // 这里可以包装一个函数，传进来的是 ttf_file, 这样还可以检查 index 的合法性 
        IntPtr nthGlyphPtr = GetGlyphPtrAtIndex(glyphsPtr, index);
        ttf_glyph glyph = Marshal.PtrToStructure<ttf_glyph>(nthGlyphPtr);
        return glyph;
    }

    public static IntPtr GetGlyphPtrAtIndex(IntPtr glyphsPtr,int index)
    {
        int glyphSize = Marshal.SizeOf<ttf_glyph>();
        IntPtr nthGlyphPtr = new IntPtr(glyphsPtr.ToInt64() + index * glyphSize);
        return nthGlyphPtr;
    }

    public static ttf_mesh3d GenerateMesh3D(IntPtr glyphPtr, byte quality, int features, float depth)
    {
        IntPtr mesh3dPtr = GenerateMesh3DPtr(glyphPtr, quality, features, depth);
        ttf_mesh3d mesh3d = Marshal.PtrToStructure<ttf_mesh3d>(mesh3dPtr);
        return mesh3d;
    }

    public static IntPtr GenerateMesh3DPtr(IntPtr glyphPtr,byte quality,int features,float depth)
    {
        IntPtr mesh3dPtr = IntPtr.Zero;
        int ret = ttf_glyph2mesh3d(glyphPtr,out mesh3dPtr,quality,features,depth);
        return mesh3dPtr;
    }

}

}

