using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIContainer : MonoBehaviour
{
    Image _buffImage;
    Image _disableImage;
    TextMeshProUGUI _countText;
    int _count = 1;
    int Count
    {
        get { return _count; }
        set
        {
            if (_count == value)
                return;
            _count = value;
            _countText.text = _count > 1 ? _count.ToString() : "";
        }
    }
    bool _isInit = false;
    void Init()
    {
        if (_isInit)
            return;
        _buffImage = transform.FindChild<Image>(false, "BuffImg");
        _disableImage = transform.FindChild<Image>(true, "DisableImg");
        _countText = transform.FindChild<TextMeshProUGUI>(true);
        _isInit = true;
    }
    public void Set(ItemBuffData data)
    {
        Init();
        _buffImage.sprite = data.Icon;
        _disableImage.fillAmount = 0f;
    }
    private void OnEnable()
    {
        Init();
        Count = 1;
        _disableImage.fillAmount = 0f;
    }
    public void SetFillAmount(float val)
    {
        _disableImage.fillAmount = val;
    }
    public void CountBuff()
    {
        Count++;
    }
}
