using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public UnityEvent OnClicked;

    private Vector3 btuScale;
    private Tween currentTweenA;
    private Tween currentTweenB;

    private bool on = false;

    void Start()
    {
        btuScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        KillTween();

        Debug.Log("호버");
        //transform.localScale = btuScale * 1.05f;
        currentTweenA = transform.DOScale(0.75f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        KillTween();

        Debug.Log("나감");
        //transform.localScale = btuScale;
        currentTweenA = transform.DOScale(0.7f, 0.2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"누름 {on}");
        if (on) return;
        on = true;

        if (currentTweenB != null && currentTweenB.IsActive())
        {
            currentTweenB.Kill();
        }

        currentTweenB = transform.DOPunchScale(new Vector3(-0.05f, -0.05f, -0.05f), 0.3f, 10, 1)
            .OnComplete(() => {
                 on = false;
                OnClicked?.Invoke();
            });
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("뗌");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        

        Debug.Log("이벤트");
        
        // 이벤트
    }

    private void KillTween()
    {
        if (currentTweenA != null && currentTweenA.IsActive())
        {
            currentTweenA.Kill();
        }
    }
}