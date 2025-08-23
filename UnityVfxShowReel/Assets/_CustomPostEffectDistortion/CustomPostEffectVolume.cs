using UnityEngine;
using UnityEngine.Rendering;

namespace ayy.CustomPostEffectDistortion
{
    public class CustomPostEffectVolume : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter isActive = new BoolParameter(true);
        [Header("Mode")]
        public ClampedIntParameter Mode = new ClampedIntParameter(0, 0, 2);
        public BoolParameter debugDistortionStrength = new BoolParameter(false);

        [Header("Wave Mode")] 
        public ClampedFloatParameter WaveFreq = new ClampedFloatParameter(1.0f,-5.0f,5.0f);
        public ClampedFloatParameter WaveAmplitude = new ClampedFloatParameter(1.0f, -5.0f, 5.0f);
        
        [Header("Zoomer Mode")]
        public ClampedFloatParameter ZoomerInner = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
        public ClampedFloatParameter ZoomerOuter = new ClampedFloatParameter(0.3f, 0.001f, 1.0f);
        public ClampedFloatParameter ZoomerZoomFactor = new ClampedFloatParameter(0.1f, 0.0f, 10.0f);
        
        [Header("Auto Expand Mode")]
        public ClampedFloatParameter LowerThreshold = new ClampedFloatParameter(0.2f, 0.0f, 5.0f);
        public ClampedFloatParameter IncThreshold = new ClampedFloatParameter(0.3f, 0.0f, 1.0f);
        public ClampedFloatParameter ZoomFactor = new ClampedFloatParameter(0.2f, 0.0f, 3.0f);
        
        [Header("Grid Generator")]
        public ClampedFloatParameter cellsNum = new ClampedFloatParameter(1.0f, 1.0f, 20.0f);
        public ClampedFloatParameter gridBorderSize = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);
        public ColorParameter lineColor = new ColorParameter(Color.white);
        public ColorParameter bgColor1 = new ColorParameter(Color.red);
        public ColorParameter bgColor2 = new ColorParameter(Color.yellow);

        public bool IsActive()
        {
            return isActive.value;
        }
    }
}
