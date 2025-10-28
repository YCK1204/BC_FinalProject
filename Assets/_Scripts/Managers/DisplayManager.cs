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
    private const string _fadeIn2 = "on_UI2";
    private const string _fadeOut = "off_UI";
    private const string _fadeOut2 = "off_UI2";


    private void Awake()
    {
        Instance = this;
    }


    //픽셀 퍼펙트 카메라 키고 끄기 SetPPC(false) 카메라 연출 사용전에 켜주면 됩니다.
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

    //보스 레이드 연출때 미니맵,hp바 안보이게 하기
    public void HubFadeOut()
    {
        _uiAnimator.Play(_hubOut);
        Debug.Log("HUB 끄기");
    }

    //hub 키기
    public void HubFadeIn()
    {
        _uiAnimator.Play(_hubIn);
        Debug.Log("HUB 켜기");
    }

    // 클리어쪽 모션 미추가 - 보스 클리어후,호출
    // 별도로 보스시작 연출처럼 3~4초정도 보스 기준으로 카메라 잡아주는 연출도 같이 넣어두면 좋을거같음
    public void PlayClearEffect()
    {
        StartCoroutine(SetPPCDelay(false, 0f));
        StartCoroutine(ClearCrt());

        PlayerManager.Instance.Player.OnUI = false;
    }

    private IEnumerator ClearCrt()
    {
        _uiAnimator.Play(_Clear);

        yield return new WaitForSeconds(4f);
        _fadeAnimator.Play(_fadeIn);
        yield return new WaitForSeconds(3f);
        _fadeAnimator.Play(_fadeIn2);
        yield return new WaitForSeconds(2f);
        MapManager.Instance.InitFloor();
        _fadeAnimator.Play(_fadeOut2);
        yield return new WaitForSeconds(1f);
        PlayerManager.Instance.Player.OnUI = true;
    }

    public void EndClear()
    {
        _fadeAnimator.Play(_fadeOut);
    }
}