using UnityEngine;
using UnityEngine.Rendering;

namespace ayy
{
    public class SetupRenderPipeline : MonoBehaviour
    {
        [SerializeField] private RenderPipelineAsset _renderPipelineAsset = null;
     
        void Start()
        {
            //GraphicsSettings.defaultRenderPipeline = _renderPipelineAsset;
        }
        
        void Update()
        {
        
        }
    }
}

