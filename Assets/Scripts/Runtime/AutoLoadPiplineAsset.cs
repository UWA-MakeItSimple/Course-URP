using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class AutoLoadPiplineAsset : MonoBehaviour
{
    public UniversalRenderPipelineAsset _UniversalRenderPipelineAsset;

    private void OnEnable()
    {
        UpdatePiplineAsset();
    }

    private void UpdatePiplineAsset()
    {
        if (_UniversalRenderPipelineAsset)
        {
            GraphicsSettings.renderPipelineAsset = _UniversalRenderPipelineAsset;
            //Debug.Log(_UniversalRenderPipelineAsset);
        }
    }
}