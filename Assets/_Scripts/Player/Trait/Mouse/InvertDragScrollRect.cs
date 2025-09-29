using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvertDragScrollRect : ScrollRect
{
    [Tooltip("씬 시작 시 현재 위치를 하한선으로 잡습니다.")]
    public bool captureStartAsMin = true;

    [Range(0f, 1f)]
    [Tooltip("세로 정규화 위치 하한(0=바닥, 1=맨 위). 시작 시점 값으로 자동 설정 가능.")]
    public float minVerticalNormalized = 0f;

    protected override void Start()
    {
        base.Start();
        if (captureStartAsMin)
            minVerticalNormalized = verticalNormalizedPosition;
    }

    public override void OnScroll(PointerEventData data)
    {
        base.OnScroll(data);
        ClampToMin();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        ClampToMin();
    }

    void LateUpdate()
    {
        ClampToMin();
    }

    void ClampToMin()
    {
        if (vertical)
            verticalNormalizedPosition = Mathf.Max(minVerticalNormalized, verticalNormalizedPosition);
    }
}
