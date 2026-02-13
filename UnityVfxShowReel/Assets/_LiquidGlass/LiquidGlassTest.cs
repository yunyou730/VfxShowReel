using UnityEngine;


namespace ayy
{
    public class LiquidGlassTest : MonoBehaviour
    {
        public Transform[] _globalRotateObjects = null;
        public Transform[] _localRotateObjects = null;
        public Material _glassMaterial = null;
        
        void Update()
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
