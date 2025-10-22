using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class UIButtonOut : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private UnityEvent OnClicked;

    [SerializeField] private Image buttonImage;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.red;

    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.localPosition;

        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = GetComponentInChildren<Image>();
            }
        }

        SetImageColor(normalColor);
    }

    private void OnEnable()
    {
        SetImageColor(normalColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetImageColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetImageColor(normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localPosition = originalPos - new Vector3(0, 3f, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localPosition = originalPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }

    private void SetImageColor(Color color)
    {
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
        else
        {
            Debug.LogWarning("UIButtonOut 비었음");
        }
    }
}