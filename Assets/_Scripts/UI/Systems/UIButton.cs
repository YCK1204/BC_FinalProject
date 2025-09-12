using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public UnityEvent OnClicked;

    private Vector3 btuScale;
    private Tween currentTween;

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
        currentTween = transform.DOScale(0.8f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        KillTween();

        Debug.Log("나감");
        //transform.localScale = btuScale;
        currentTween = transform.DOScale(0.7f, 0.2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (on) return;
        on = true;

        KillTween();

        Debug.Log("누름");

        currentTween = transform.DOPunchScale(new Vector3(-0.1f, -0.1f, -0.1f), 0.3f, 10, 1)
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
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
    }
}