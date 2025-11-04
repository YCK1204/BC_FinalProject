using DG.Tweening;
using Game.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventController : MonoBehaviour
{
    [SerializeField]
    private Game.Player.PlayerCharacter _player;

    [SerializeField]
    private float _duration = 3f;

    [SerializeField]
    private Vector2 _direction = new Vector2(1, 0);

    [Header("Event")]
    public GameObject[] objects;

    private int _currentIndex = 0;

    [Header("Flash")]
    [SerializeField] private ScreenFlash _flashEffect;
    private float _flashDuration = 0.2f;

    [Header("Player Glitch")]
    [SerializeField] private Material _glitchMaterial;
    private string _glitchProperty = "_GlitchIntensity";
    private string _glitchJump = "_VerticalJump";
    private string _glitchShake = "_HorizontalShake";
    private float _glitchValue = 0f;
    private float _glitchJumpValue = 0f;

    [Header("Animator")]
    [SerializeField] private Animator _animator;

    private IEnumerator Start()
    {

        _player.AutoMove(_duration, _direction.normalized);
        _glitchMaterial.SetFloat(_glitchProperty, 0f);
        _glitchMaterial.SetFloat(_glitchJump, 0f);
        _glitchMaterial.SetFloat(_glitchShake, 0f);

        yield return new WaitForSeconds(3f);
        OnTriggerActive(0);

        Manager.Audio.SetBgm(AudioKey.BGM.BGM_Intro);

    }

    public void OnTriggerActive(int triggerIndex)
    {
        if (triggerIndex != _currentIndex) return;

        if (_currentIndex >= 5 && _flashEffect != null)
        {
            _flashEffect.Flash(_flashDuration);
            _flashDuration += 0.12f;

            _glitchValue += 0.25f;
            _glitchMaterial.SetFloat(_glitchProperty, _glitchValue);

            _glitchJumpValue += 0.25f;
            _glitchMaterial.SetFloat(_glitchJump, _glitchJumpValue);

            _glitchMaterial.SetFloat(_glitchShake, 1f);
        }

        if (_currentIndex - 1 >= 0 && _currentIndex - 1 < objects.Length)
        {
            objects[_currentIndex - 1].SetActive(false);
        }

        if (_currentIndex < objects.Length)
        {
            objects[_currentIndex].SetActive(true);
        }

        if (_currentIndex == 8)
        {
            StartCoroutine(End());
        }

        _currentIndex++;
    }

    private IEnumerator End()
    {
        DOTween.Restart("phonshake1");
        yield return new WaitForSeconds(3f);
        _animator.SetTrigger("SetStart");
        DOTween.Restart("phonshake2");

        yield return new WaitForSeconds(3f);
        Manager.Data.playerSOData.IsIntroCompleted = true;
        PlayerPrefs.SetInt("IsIntroCompleted", 1);
        Manager.Analytics.SendFunnelStep(FunnelStep._StageC, 2);

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Main");
    }

    //void Update()
    //{
    //    //테스트
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        DOTween.Restart("phonshake");
    //        Debug.Log("스페이스");
    //    }
    //}
}
