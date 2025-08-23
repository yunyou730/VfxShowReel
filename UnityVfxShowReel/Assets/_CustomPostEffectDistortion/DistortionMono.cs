using UnityEngine;
using UnityEngine.Rendering;

namespace ayy.CustomPostEffectDistortion
{
    public enum ECustomDistortionMode
    {
        AutoExpanding,
        Zoomer,
        SinWave,
    }

    public class DistortionMono : MonoBehaviour
    {
        [SerializeField] private DistortionData _data;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _expandDuration = 2.0f;
        
        private float _expandElapsed = 0.0f;
        private bool _expanding = false;

        private float _startValue = 0.0f;
        public float DestValue = 4.0f;
        
        void Update()
        {
            var stack = VolumeManager.instance.stack;
            if (stack == null)
            {
                return;
            }

            var volume = stack.GetComponent<CustomPostEffectVolume>();
            if (volume.IsActive())
            {
                if (volume.Mode.value == (int)ECustomDistortionMode.AutoExpanding)
                {
                    if(Input.GetMouseButtonUp(0))
                    {
                        OnStartDistortion();
                    }
                    if (_expanding)
                    {
                        OnUpdateDistortion();
                    }
                }
                else if (volume.Mode.value == (int)ECustomDistortionMode.Zoomer || volume.Mode.value == (int)ECustomDistortionMode.SinWave)
                {
                    if (Input.GetMouseButton(0))
                    {
                        _data.CenterX = Input.mousePosition.x / Screen.width;
                        _data.CenterY = Input.mousePosition.y / Screen.height;
                    }
                }
            }

            float scrollDelta = Input.mouseScrollDelta.y * Time.deltaTime;
            _data.TestScrollDelta += scrollDelta;
        }

        private void OnStartDistortion()
        {
            _expanding = true;
            
            _expandElapsed = 0.0f;
            _data.CenterX = Input.mousePosition.x / Screen.width;
            _data.CenterY = Input.mousePosition.y / Screen.height;
            var stack = VolumeManager.instance.stack;
            var volume = stack.GetComponent<CustomPostEffectVolume>();
            if (volume.IsActive())
            {
                _data.ZoomFactor = volume.ZoomFactor.value;
                _data.LowerThreshold = volume.LowerThreshold.value;
                _data.IncThreshold = volume.IncThreshold.value;
                // _data.DecThreshold = volume.DecThreshold.value;

                _startValue = volume.LowerThreshold.value;;
            }
        }

        private void OnStopDistortion()
        {
            // _expanding = false;
            //_data.ZoomFactor = 0.0f;
        }

        private void OnUpdateDistortion()
        {
            _expandElapsed += Time.deltaTime;
            float pct = _expandElapsed / _expandDuration;
            pct = Mathf.Clamp(pct, 0.0f, 1.0f);
            
            var stack = VolumeManager.instance.stack;
            var volume = stack.GetComponent<CustomPostEffectVolume>();
            if (volume.IsActive())
            {
                _data.ZoomFactor = volume.ZoomFactor.value;
                _data.IncThreshold = volume.IncThreshold.value;
                _data.LowerThreshold = Mathf.Lerp(_startValue, DestValue, pct);
            }
        }
    }
    
}

