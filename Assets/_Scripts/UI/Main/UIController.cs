using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("UI")]
    public Animator Animator;

    [Header("화면 등록")]
    public UIScreen[] screens;

    private Dictionary<string, UIScreen> screenMap;
    private UIScreen currentScreen;

    private Dictionary<UiState, UiStateBase> stateMap;
    private UiStateBase activeState;
    public UiState CurrentState { get; private set; }
    public UiState PreviousState { get; private set; }
    private UiState nextState;

    private bool isState = false;

    private void Awake()
    {
        // 초기화
        stateMap = new Dictionary<UiState, UiStateBase>
        {
            { UiState.Hidden, new HiddenState(this) },
            { UiState.NormalView, new NormalViewState(this) },
            { UiState.WideView, new WideViewState(this) },
            { UiState.FullScreen, new FullScreenState(this) },
        };

        // 화면 등록
        screenMap = new Dictionary<string, UIScreen>();
        foreach (UIScreen s in screens)
        {
            s.panel.SetActive(false);
            screenMap[s.name] = s;
        }

        ChangeState(UiState.Hidden);
    }

    private void Update()
    {
        if (isState) return;

        // Test
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log($"esc - {CurrentState}");
            if (CurrentState == UiState.NormalView)
            {
                HideUI();
            }
            else
            {
                ShowNormalView();
            }
                
        }
    }

    public void ChangeState(UiState state)
    {
        if (CurrentState == state || isState)
            return;

        StartCoroutine(ChangeStateRoutine(state));
    }

    private IEnumerator ChangeStateRoutine(UiState newState)
    {
        isState = true;

        activeState?.Exit();

        PreviousState = CurrentState;
        CurrentState = newState;
        activeState = stateMap[newState];
        activeState.Enter();

        yield return new WaitForSeconds(0.3f);

        isState = false;
    }

    public void ShowScreen(string screenName)
    {
        if (string.IsNullOrEmpty(screenName))
        {
            Debug.LogError("!빈값");
            return;
        }

        if (!screenMap.TryGetValue(screenName, out UIScreen screen))
        {
            Debug.LogError($"!없는 스크린");
            return;
        }

        if (currentScreen == screen)
        {
            Debug.Log($"!같은 스크린");
            return;
        }

        // 스크린 끄기
        if (currentScreen != null && currentScreen.panel != null)
        {
            currentScreen.panel.SetActive(false);
        }

        // 스크린 켜기
        currentScreen = screen;
        if (currentScreen.panel != null)
        {
            currentScreen.panel.SetActive(true);
        }
    }

    public void ShowNormalView() => ChangeState(UiState.NormalView);
    public void ShowWideView() => ChangeState(UiState.WideView);
    public void ShowFullScreen() => ChangeState(UiState.FullScreen);
    public void HideUI() => ChangeState(UiState.Hidden);
}