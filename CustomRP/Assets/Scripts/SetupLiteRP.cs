using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SetupLiteRP : MonoBehaviour
{
    [SerializeField] RenderPipelineAsset _currentRenderPipeline;
    void OnEnable()
    {
        GraphicsSettings.defaultRenderPipeline = _currentRenderPipeline;
    }

    private void OnValidate()
    {
        GraphicsSettings.defaultRenderPipeline = _currentRenderPipeline;
    }
}
