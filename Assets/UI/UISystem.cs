using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UISystem : MonoBehaviour
{
    public Camera UICamera;

    public void ChangeRP0()
    {
        UICamera.GetUniversalAdditionalCameraData().SetRenderer(0);
    }


    public void ChangeRP1()
    {
        UICamera.GetUniversalAdditionalCameraData().SetRenderer(1);
    }
}
