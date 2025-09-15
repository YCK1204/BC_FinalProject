using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIButtonOut : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private UnityEvent OnClicked;

    [SerializeField] private Image buttonImage;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.red;

    private Vector3 originalPos;
    private Material buttonMat;
    private Material buttonOutMat;

    private void Awake()
    {
        if (buttonImage) 
            buttonMat = Instantiate(buttonImage.material);

        if (buttonImage) 
            buttonImage.material = buttonMat;
    }

    private void Start()
    {
        originalPos = transform.localPosition;
        SetMaterialColor(normalColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetMaterialColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetMaterialColor(normalColor);
    }

    public void OnPointerDown(PointerEventData eventData) {
        transform.localPosition = originalPos - new Vector3(0, 3f, 0);
    }
    public void OnPointerUp(PointerEventData eventData) {
        transform.localPosition = originalPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }

    private void SetMaterialColor(Color color)
    {
        if (buttonMat) buttonMat.SetColor("_Color", color);
    }
}