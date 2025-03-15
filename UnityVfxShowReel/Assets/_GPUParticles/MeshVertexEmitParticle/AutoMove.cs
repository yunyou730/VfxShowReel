using UnityEngine;

namespace ayy
{
    public class AutoMove : MonoBehaviour
    {
        private float _elapsedTime = 0.0f;
        
        private float _elapsedForZ = 0.0f;
        public float _zCircleTime = 2.0f;
        public float _zSpeed = 1.0f;
        private bool _zDir = true;


        //private float _elapsedForXY = 0.0f;
        private float _xyCirlceCurrentAngle = 0.0f;
        public float _xyCircleSpeed = Mathf.PI;
        public float _xyRadius = 10.0f;

        private float _elapsedForRadius = 0.0f;
        public float _radiusChangeSpeed = 1.0f;
        public float _radiusChangeTime = 1.0f;
        private bool _radiusDir = true;

        void Start()
        {

        }

        void Update()
        {
            _elapsedTime += Time.deltaTime;
            
            float z = UpdateForZ(Time.deltaTime);

            //Vector3 currentPos = transform.position;
            float x, y;
            UpdateForXY(Time.deltaTime,out x,out y);
            transform.position = new Vector3(x,y,z);
        }

        private float UpdateForZ(float dt)
        {
            _elapsedForZ += dt;
            if (_elapsedForZ >= _zCircleTime)
            {
                _zDir = !_zDir;
                _elapsedForZ -= _zCircleTime;
            }

            float dis = _zDir ? _zSpeed * dt : -_zSpeed * dt;
            float nextZ = transform.position.z + dis;
            return nextZ;
        }

        private void UpdateForXY(float dt,out float x,out float y)
        {
            // change radius
            _elapsedForRadius += dt;
            if (_elapsedForRadius >= _radiusChangeTime)
            {
                _elapsedForRadius -= _radiusChangeTime;
                _radiusDir = !_radiusDir;
            }
            float deltaRadius = _radiusDir ? _radiusChangeSpeed * dt : - _radiusChangeSpeed * dt;
            _xyRadius += deltaRadius;

            
            // xy move by circle
            Vector3 origin = Vector3.zero;
            _xyCirlceCurrentAngle += _xyCircleSpeed * dt;            
            y = _xyRadius * Mathf.Sin(_xyCirlceCurrentAngle);
            x = _xyRadius * Mathf.Cos(_xyCirlceCurrentAngle);
        }

    }

}
