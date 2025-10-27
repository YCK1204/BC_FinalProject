using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class DisplayManager : MonoBehaviour
{
    public static DisplayManager Instance;

    [SerializeField] private Animator _uiAnimator;
    [SerializeField] private Animator _fadeAnimator;

    [SerializeField] private PixelPerfectCamera _pixelPerfectCamera;

    private const string _hubIn = "all_on";
    private const string _hubOut = "all_off";
    private const string _Clear = "clear";
    private const string _fadeIn = "on_UI";
    private const string _fadeOut = "off_UI";


    private void Awake()
    {
        Instance = this;
    }

    public void SetPPC(bool a , float b = 0f)
    {
        StartCoroutine(SetPPCDelay(a, b));
    }

    private IEnumerator SetPPCDelay(bool a, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        _pixelPerfectCamera.enabled = a;
    }

    public void HubFadeOut()
    {
        _uiAnimator.SetTrigger(_hubOut);
        Debug.Log("HUB 끄기");
    }

    public void HubFadeIn()
    {
        _uiAnimator.SetTrigger(_hubIn);
        Debug.Log("HUB 켜기");
    }

    public void PlayClearEffect()
    {
        StartCoroutine(ClearCrt());
    }

    private IEnumerator ClearCrt()
    {
        _uiAnimator.SetTrigger(_Clear);

        yield return new WaitForSeconds(4f);

        _fadeAnimator.SetTrigger(_fadeIn);
    }

    public void EndClear()
    {
        _fadeAnimator.SetTrigger(_fadeOut);
    }
}