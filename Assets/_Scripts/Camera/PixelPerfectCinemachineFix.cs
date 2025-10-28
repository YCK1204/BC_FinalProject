using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class PixelPerfectCinemachineFix : MonoBehaviour
{
    public PixelPerfectCamera pixelPerfectCamera;

    void LateUpdate()
    {
        if (pixelPerfectCamera == null)
            return;

        float unitsPerPixel = 1f / pixelPerfectCamera.assetsPPU;
        Vector3 pos = transform.position;

        pos.x = Mathf.Round(pos.x / unitsPerPixel) * unitsPerPixel;
        pos.y = Mathf.Round(pos.y / unitsPerPixel) * unitsPerPixel;

        transform.position = pos;
    }
}
