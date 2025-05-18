using System;
using System.Runtime.InteropServices;
using UnityEngine;
using ayy.ttf;

public class Glyph3DTest : MonoBehaviour
{
    
    void Start()
    {
        IntPtr ttfPointer = IntPtr.Zero;
        Debug.Log("ttfPtr_before:" + ttfPointer.ToString("X"));
        
        //TTF2MeshNativeLibrary.ttf_file ttf = (Marshal.PtrToStructure<TTF2MeshNativeLibrary.ttf_file>(ttfPointer));
        
        string path = "/Users/miaoyunlong/Documents/miao_coding/VfxShowReel/UnityVfxShowReel/Assets/_Glyph3D/jijiguowang.ttf";
        TTF2MeshNativeLibrary.LoadTTFFile(path,out ttfPointer,false);
        
        ttf_file ttf = (Marshal.PtrToStructure<ttf_file>(ttfPointer));
        Debug.Log("ttf nchars:" + ttf.nchars + " nglyphs:" + ttf.nglyphs);

        ttf_glyph glyph = TTF2MeshNativeLibrary.GetGlyphAtIndex(ttf.glyphs,115);
        
        Debug.Log("ttfPtr_after:" + ttfPointer.ToString("X"));
        
        //int ttf_glyph2mesh3d(ttf_glyph_t *glyph, ttf_mesh3d_t **output, uint8_t quality, int features, float depth);
        
        IntPtr glyphPtr = TTF2MeshNativeLibrary.GetGlyphPtrAtIndex(ttf.glyphs,115);
        IntPtr mesh3dPtr = IntPtr.Zero;
        
        /*
        #define TTF_QUALITY_LOW    10     // default quality value for some functions
        #define TTF_QUALITY_NORMAL 20     // default quality value for some functions
        #define TTF_QUALITY_HIGH   50     // default quality value for some functions
        #define TTF_FEATURES_DFLT   0     // default value of ttf_glyph2mesh features parameter
        #define TTF_FEATURE_IGN_ERR 1     // flag of ttf_glyph2mesh to ignore uncritical mesh errors
        */
        ttf_mesh3d mesh3d = TTF2MeshNativeLibrary.GenerateMesh3D(glyphPtr,20,0,15.0f);
        

        TTF2MeshNativeLibrary.FreeTTFFile(ttfPointer);
    }
    
    void Update()
    {
        
    }

    void onDestroy()
    {
        
    }
}
