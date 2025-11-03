using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioKey;

public class UIController : MonoBehaviour
{
    [Header("UI")]
    public Animator Animator;
    [SerializeField] private Animator _uiFade;

    [Header("화면 등록")]
    public UIScreen[] screens;

    private Dictionary<string, UIScreen> _screenMap;
    private UIScreen _currentScreen;

    private Dictionary<UiState, UiStateBase> _stateMap;
    private UiStateBase _activeState;
    public UiState CurrentState { get; private set; }
    public UiState PreviousState { get; private set; }
    private UiState _nextState;

    private bool _isState = false;

    private void Awake()
    {
        DOTween.Init();
        DOTween.defaultUpdateType = UpdateType.Normal;
        DOTween.defaultTimeScaleIndependent = true;

        // 초기화
        _stateMap = new Dictionary<UiState, UiStateBase>
        {
            { UiState.Hidden, new HiddenState(this) },
            { UiState.Main, new MainState(this) },
            { UiState.Stats, new CharacterStatsState(this) },
            { UiState.Setting, new SettingsState(this) },
        };

        // 화면 등록
        _screenMap = new Dictionary<string, UIScreen>();
        foreach (UIScreen s in screens)
        {
            s.panel.SetActive(false);
            _screenMap[s.name] = s;
        }

        ChangeState(UiState.Hidden);
    }

    private void Update()
    {
        if (_isState) return;

        if (!PlayerManager.Instance.Player.OnUI && !PlayerManager.Instance.Player.IsDead)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log($"esc - {CurrentState}");
                if (CurrentState == UiState.Main)
                {
                    _uiFade.Play("off_UI", 0, 0f);
                    HideUI();
                    Time.timeScale = 1f;
                    DisplayManager.Instance.SetPPC(true, 0.5f);
                }
                else if (CurrentState == UiState.Hidden)
                {
                    _uiFade.Play("on_UI", 0, 0f);
                    Animator.Play("show", 0, 0f);
                    Animator.SetInteger("State", 1);
                    PlayerManager.Instance.Player.Animator.Play("Phon");
                    ShowNormalView();
                    DisplayManager.Instance.SetPPC(false);
                }
                else
                {
                    BackUI();
                }
            }
            else if (Input.GetMouseButtonDown(0) && CurrentState == UiState.Hidden)
            {
                Debug.Log($"mouse click to open - {CurrentState}");
                _uiFade.Play("on_UI", 0, 0f);
                Animator.Play("show", 0, 0f);
                Animator.SetInteger("State", 1);
                PlayerManager.Instance.Player.Animator.Play("Phon");
                ShowNormalView();
                DisplayManager.Instance.SetPPC(false);
            }
            else if (CurrentState != UiState.Main && CurrentState != UiState.Hidden)
            {
                // BackUI();
            }
        }
    }

    public void ChangeState(UiState state)
    {
        if (CurrentState == state || _isState)
            return;

        StartCoroutine(ChangeStateRoutine(state));
    }

    private IEnumerator ChangeStateRoutine(UiState newState)
    {
        _isState = true;

        _activeState?.Exit();

        PreviousState = CurrentState;
        CurrentState = newState;
        _activeState = _stateMap[newState];
        _activeState.Enter();

        if ((PreviousState == UiState.Hidden && newState == UiState.Main) ||
            (PreviousState == UiState.Main && newState == UiState.Hidden))
        {
            yield return new WaitForSecondsRealtime(0.5f);
            
        }

        _isState = false;
    }

    public void ShowScreen(string screenName)
    {
        if (string.IsNullOrEmpty(screenName))
        {
            Debug.LogError("!빈값");
            return;
        }

        if (!_screenMap.TryGetValue(screenName, out UIScreen screen))
        {
            return;
        }

        if (_currentScreen == screen)
        {
            Debug.Log($"!같은 스크린");
            return;
        }

        // 스크린 끄기
        if (_currentScreen != null && _currentScreen.panel != null)
        {
            _currentScreen.panel.SetActive(false);
        }

        // 스크린 켜기
        _currentScreen = screen;
        if (_currentScreen.panel != null)
        {
            _currentScreen.panel.SetActive(true);
        }
    }

    public void ShowNormalView() => ChangeState(UiState.Main);
    public void ShowStatsUI() => ChangeState(UiState.Stats);
    public void HideUI() => ChangeState(UiState.Hidden);
    public void ShowSettingUI() => ChangeState(UiState.Setting);
    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackUI()
    {
        switch (CurrentState)
        {
            case UiState.Main:
                _uiFade.Play("off_UI", 0, 0f);
                ChangeState(UiState.Hidden);
                Time.timeScale = 1f;
                DisplayManager.Instance.SetPPC(true, 0.5f);
                break;

            case UiState.Stats:
                ChangeState(UiState.Main);
                break;

            case UiState.Setting:
                ChangeState(UiState.Main);
                break;

            case UiState.Hidden:
                break;
        }
    }

    public void TimeStop()
    {
        Time.timeScale = 0f;
    }
}