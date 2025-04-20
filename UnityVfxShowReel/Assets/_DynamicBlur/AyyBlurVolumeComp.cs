using UnityEngine;
using UnityEngine.Rendering;


namespace ayy.rendering
{
    
    public class AyyBlurVolumeComp : VolumeComponent, IPostProcessComponent
    {
        [Header("Blur")]
        
        [Tooltip("is enable")]
        public BoolParameter enable = new BoolParameter(false);
        
        [Tooltip("test1")]
        public MinFloatParameter test1 = new MinFloatParameter(0.9f, 0f);
        
        [Tooltip("test2")]
        public MinFloatParameter test = new MinFloatParameter(0f, 0f);
        //
        // /// <summary>
        // /// Tells if the post process needs to be rendered or not.
        // /// </summary>
        // /// <returns><c>true</c> if the effect should be rendered, <c>false</c> otherwise.</returns>
        // public bool IsActive()
        // {
        //     return true;
        // }

        public bool IsActive()
        {
            return enable.value;
        }
    }
    
}
