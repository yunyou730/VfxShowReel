#include <iostream>
#include "../ttf2mesh.h"


int main(int argc, const char * argv[]) {
    // insert code here...
    std::cout << "Hello, World!\n";
    
    //int ttf_load_from_file(const char *filename, ttf_t **output, bool headers_only)
    
    ttf_t* ttf = nullptr;
    ttf_load_from_file("res/jijiguowang.ttf",&ttf,false);
    
    printf("font name:%s\n",ttf->names.full_name);

    
    std::wstring str = L"A";
    uint32_t ch = str[0];
    int index = ttf_find_glyph(ttf,ch);
    ttf_mesh* mesh = nullptr;
    ttf_mesh3d* mesh3d = nullptr;
    if(index >= 0)
    {
        ttf_glyph glyph = ttf->glyphs[index];
        ttf_glyph2mesh(&glyph,&mesh,TTF_QUALITY_NORMAL,TTF_FEATURES_DFLT);
        ttf_glyph2mesh3d(&glyph,&mesh3d,TTF_QUALITY_NORMAL,TTF_FEATURES_DFLT,5.0f);
    }
    
    ttf_free_mesh(mesh);
    mesh = nullptr;
    ttf_free_mesh3d(mesh3d);
    mesh3d = nullptr;
    ttf_free(ttf);
    ttf = nullptr;
    
    return 0;
}
