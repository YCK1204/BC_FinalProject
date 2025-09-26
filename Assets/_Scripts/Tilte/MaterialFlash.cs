using UnityEngine;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    [Header("Flash")]
    [SerializeField] private GameObject _flashObject;

    [Header("Mat")]
    [SerializeField] private Material _glitchMaterial;
    [SerializeField] private Material _shakeMaterial;

    private float _glitchValue = 0f;
    private float _shakeValue = 0f;

    private void Start()
    {
        _glitchMaterial.SetFloat("_GlitchIntensity", _glitchValue);
        _shakeMaterial.SetFloat("_HorizontalShake", _shakeValue);
    }

    public void Flash(float duration)
    {
        StartCoroutine(FlashCoroutine(duration));
    }

    public void FlashSet()
    {
        _flashObject.SetActive(true);

        _glitchMaterial.SetFloat("_GlitchIntensity", _glitchValue);

        _shakeValue += 0.02f;
        _shakeMaterial.SetFloat("_HorizontalShake", _shakeValue);
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        _flashObject.SetActive(true);

        _glitchValue += 0.1f;
        _glitchMaterial.SetFloat("_GlitchIntensity", _glitchValue);

        _shakeValue += 0.01f;
        _shakeMaterial.SetFloat("_HorizontalShake", _shakeValue);

        yield return new WaitForSeconds(duration);

        if (_flashObject != null)
            _flashObject.SetActive(false);
    }
}
