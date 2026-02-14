using UnityEngine;


namespace ayy
{
    public class LiquidGlassTest : MonoBehaviour
    {
        public Transform[] _globalRotateObjects = null;
        [SerializeField,Range(0,720)]private float _rotateSpeed = 90.0f;      // 自转
        [SerializeField,Range(0,720)]private float _revolutionSpeed = 90.0f;  // 公转
        
        public Material _glassMaterial = null;
        
        void Update()
        {
            UpdateObjectsRotation();
            UpdateLiquidPosByMouse();
        }

        private void UpdateObjectsRotation()
        {
            foreach (Transform t in _globalRotateObjects)
            {
                // 自转
                t.Rotate(Vector3.up,_rotateSpeed * Time.deltaTime);

                // 公转 
                Vector3 center = Vector3.zero;
                Vector3 cur = t.position;
                Vector3 dir = cur - center;
                Quaternion rot = Quaternion.AngleAxis(_revolutionSpeed * Time.deltaTime, Vector3.up);
                dir = rot * dir;
                Vector3 next = center + dir;
                t.position = next;
            }
        }

        private void UpdateLiquidPosByMouse()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                _glassMaterial.SetFloat(Shader.PropertyToID("_CenterX"),mousePos.x / Screen.width);
                _glassMaterial.SetFloat(Shader.PropertyToID("_CenterY"),mousePos.y / Screen.height);    
            }
        }
    }
    
}
