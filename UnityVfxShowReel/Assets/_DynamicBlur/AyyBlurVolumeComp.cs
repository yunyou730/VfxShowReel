using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


namespace ayy.rendering
{
    
    public class AyyBlurVolumeComp : VolumeComponent, IPostProcessComponent
    {
        [Header("Blur")]
        
        [Tooltip("is enable")]
        public BoolParameter enable = new BoolParameter(false);
        
        [Tooltip("gaussian kernel size")]
        public MinFloatParameter KernelSize = new MinFloatParameter(10f, 0f);
        
        [Tooltip("gaussian iterations")]
        public MinFloatParameter Iterations = new MinFloatParameter(2f, 0f);
        
        [Tooltip("gaussian down sample scale")]
        public ClampedFloatParameter DownSampleScale = new ClampedFloatParameter(0.5f, 0.1f, 1f);
        
        public bool IsActive()
        {
            return enable.value;
        }
    }
    
}
