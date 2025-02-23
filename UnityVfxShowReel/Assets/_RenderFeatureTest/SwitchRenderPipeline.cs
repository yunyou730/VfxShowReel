using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    public class SwitchRenderPipeline : MonoBehaviour
    {
        private UniversalAdditionalCameraData _cameraData = null;
        
        void Start()
        {
            _cameraData = GetComponent<UniversalAdditionalCameraData>();
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchURPRender(0);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                SwitchURPRender(1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                SwitchURPRender(2);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                SwitchURPRender(3);
            }            
        }

        private void SwitchURPRender(int index)
        {
            _cameraData.SetRenderer(index);
        }
    }
}

