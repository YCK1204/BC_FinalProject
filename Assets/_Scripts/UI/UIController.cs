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
    private UIScreen nextScreen;

    private Dictionary<UiState, UiStateBase> stateMap;
    private UiStateBase activeState;
    public UiState CurrentState { get; private set; }
    private UiState nextState;

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
        foreach (var s in screens)
        {
            s.panel.SetActive(false);
            screenMap[s.name] = s;
        }

        ChangeState(UiState.Hidden);
    }

    public void ChangeState(UiState newState)
    {
        if (CurrentState == newState || CurrentState == UiState.Updating)
            return;

        nextState = newState;
        CurrentState = UiState.Updating;

        activeState?.Exit();
        activeState = stateMap[newState];
        activeState.Enter();
    }

    public void OnStateTransitionComplete()
    {
        CurrentState = nextState;
    }

    public void ShowScreen(string screenName)
    {
        if (screenMap.TryGetValue(screenName, out var screen))
        {
            if (currentScreen == screen) return;

            nextScreen = screen;
            StartCoroutine(SwitchScreenCoroutine());
        }
    }

    private IEnumerator SwitchScreenCoroutine()
    {
        if (currentScreen != null)
        {
            if (currentScreen.animator != null)
            {
                currentScreen.animator.SetTrigger("Hide");
                yield return new WaitForSeconds(GetClipLength(currentScreen.animator, "Hide"));
            }
            currentScreen.panel.SetActive(false);
        }

        nextScreen.panel.SetActive(true);
        if (nextScreen.animator != null)
            nextScreen.animator.SetTrigger("Show");

        currentScreen = nextScreen;
        nextScreen = null;
    }

    private float GetClipLength(Animator animator, string clipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 0f;
    }

    public void ShowNormalView() => ChangeState(UiState.NormalView);
    public void ShowWideView() => ChangeState(UiState.WideView);
    public void ShowFullScreen() => ChangeState(UiState.FullScreen);
    public void HideUI() => ChangeState(UiState.Hidden);
}