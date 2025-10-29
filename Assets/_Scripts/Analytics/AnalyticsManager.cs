using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UnityConsent;

public enum FunnelStep
{
    None,
    _1A,
    _1B,
    _1C,
    _1D,
    _1E,
    _1F,
    _Boss,
    _StageC
}

/// <summary>
/// 사용법
/// 1. 퍼널 이벤트를 발생 시킬 코드 위치로 이동
/// 2. Manager.Analytics.SendFunnelStep(parameter, value)를 호출
/// 3. 이 때, parameter는 FunnelStep(Enum)으로 설정, 튜토리얼 ~ 베이스캠프 단계는 FunnelStep.None으로 설정
/// 4. value는 해당 표를 보고 일치하는 값으로 설정
/// 5. 끝
/// 
/// ps. 현재 튜토리얼에서는 매니저가 없는거 같은데 이거 추가해야 할 듯?
/// </summary>
public class AnalyticsManager
{
    private bool _isInitialized = false;

    public async void Init()
    {
        try
        {
            await UnityServices.InitializeAsync();

            ConsentState _consentState = new ConsentState();
            _consentState.AnalyticsIntent = ConsentStatus.Granted;
            EndUserConsent.SetConsentState(_consentState);

            _isInitialized = true;
            Debug.Log("Unity Services Initialized");

            // [PS] 초기화와 동시에 퍼널 0,1 수행
            SendFunnelStep(FunnelStep.None, 1);
            SendFunnelStep(FunnelStep._StageC, 1);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Unity Services failed to initialize: " + e.Message);
        }
    }

    /// <summary>
    /// 애널리틱스에 퍼널 이벤트를 보내는 메서드
    /// </summary>
    /// <param name="step">퍼널 종류</param>
    /// <param name="stepNumber">퍼널 진행 단계</param>
    public void SendFunnelStep(FunnelStep step, int stepNumber)
    {
        if (!_isInitialized)
        {
            Debug.LogError("Unity Services Not Initialized");
            return;
        }

        string eventName = step == FunnelStep.None ? "Funnel_Step" : "Funnel_Step" + step.ToString();
        string paramName = step == FunnelStep.None ? "Funnel_Step_Number" : "Funnel_Step_Number" + step.ToString();

        var funnelEvent = new CustomEvent(eventName); //event
        funnelEvent[paramName] = stepNumber; //parameter

        AnalyticsService.Instance.RecordEvent(funnelEvent); //custom event
        Debug.Log($"Event Send {eventName} / {paramName} / {stepNumber}");
    }
    public void SendFunnelStepForItem(bool getItem = true)
    {
        string curMapName = MapManager.Instance.CurrentMap.name;
        var lastUnderbarIdx = curMapName.LastIndexOf("_");
        if (lastUnderbarIdx != -1)
        {
            string curMap = curMapName.Substring(lastUnderbarIdx);

            FunnelStep step = MapManager.Instance.GetCurrentFunnelStep();
            int stepNum = 0;

            switch (step)
            {
                case FunnelStep.None:
                    if (getItem == false)
                        return;
                    stepNum = 12;
                    break;
                default:
                    step = Enum.Parse<FunnelStep>(curMap);
                    stepNum = getItem ? 5 : 4;
                    break;
            }
            SendFunnelStep(step, stepNum);
        }
    }
}
