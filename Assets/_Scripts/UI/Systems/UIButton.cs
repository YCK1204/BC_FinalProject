using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public UnityEvent OnClicked;

    private Vector3 initialScale;
    private Tween currentTweenA;
    private Tween currentTweenB;
    private Tween textFadeTween;

    private bool on = false;
    private Text buttonText;

    void Awake()
    {
        initialScale = transform.localScale;

        Transform textChild = transform.Find("text");
        if (textChild != null)
        {
            buttonText = textChild.GetComponent<Text>();
            if (buttonText != null)
            {
                Color tempColor = buttonText.color;
                tempColor.a = 0f;
                buttonText.color = tempColor;
            }
        }
    }

    void OnEnable()
    {
        transform.localScale = initialScale;

        if (buttonText != null)
        {
            Color tempColor = buttonText.color;
            tempColor.a = 0f;
            buttonText.color = tempColor;
        }

        KillTween();
        KillTextFadeTween();
        if (currentTweenB != null && currentTweenB.IsActive()) currentTweenB.Kill();
        on = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        KillTween();
        KillTextFadeTween();

        Debug.Log("호버");
        currentTweenA = transform.DOScale(initialScale * 1.05f, 0.2f);

        if (buttonText != null)
        {
            textFadeTween = buttonText.DOColor(new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 1f), 0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        KillTween();
        KillTextFadeTween();

        Debug.Log("나감");
        currentTweenA = transform.DOScale(initialScale, 0.2f);

        if (buttonText != null)
        {
            textFadeTween = buttonText.DOColor(new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 0f), 0.2f);
        }
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

        currentTweenB = transform.DOPunchScale(initialScale * -0.05f, 0.3f, 10, 1)
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
    }

    private void KillTween()
    {
        if (currentTweenA != null && currentTweenA.IsActive())
        {
            currentTweenA.Kill();
        }
    }

    private void KillTextFadeTween()
    {
        if (textFadeTween != null && textFadeTween.IsActive())
        {
            textFadeTween.Kill();
        }
    }
}