using UnityEngine;
using UnityEngine.Rendering;

namespace ayy.CustomPostEffectDistortion
{
    public class DistortionMono : MonoBehaviour
    {
        [SerializeField] private DistortionData _data;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _expandDuration = 2.0f;
        
        private float _expandElapsed = 0.0f;
        private bool _expanding = false;

        private float _startValue = 0.0f;
        public float DestValue = 4.0f;
        
        void Start()
        {
            
        }

        void Update()
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
                _data.DecThreshold = volume.DecThreshold.value;

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
                //_data.LowerThreshold = volume.LowerThreshold.value;
                _data.IncThreshold = volume.IncThreshold.value;
                _data.DecThreshold = volume.DecThreshold.value;

                _data.LowerThreshold = Mathf.Lerp(_startValue, DestValue, pct);
            }
            // float prevPct = _expandElapsed / _expandDuration;
            // _expandElapsed += Time.deltaTime;
            //
            // float pct = _expandElapsed / _expandDuration;
            // pct = Mathf.Clamp(pct, 0.0f, 1.0f);
            
            // _expandElapsed += Time.deltaTime;
            // float pct = Mathf.Sin(_expandElapsed * Mathf.PI * 2.0f);
            // pct = pct * 0.5f + 0.5f;
            //
            // var stack = VolumeManager.instance.stack;
            // var volume = stack.GetComponent<CustomPostEffectVolume>();
            // if (volume.IsActive())
            // {
            //     float display = _curve.Evaluate(pct);
            //     _data.ZoomFactor = Mathf.Lerp(volume.srcZoomFactor.value, volume.dstZoomFactor.value, display);
            //     _data.LowerThreshold = Mathf.Lerp(volume.srcLowerThreshold.value, volume.dstLowerThreshold.value, display);
            //     _data.IncTreshold = Mathf.Lerp(volume.srcIncThreshold.value, volume.dstIncThreshold.value, display);
            // }
        }
    }
    
}

