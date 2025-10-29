using Game.Player;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AudioKey;

public class Buff : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    BuffUI BuffUI;
    ItemBuffData _data;
    Image _buffImage;
    Image _disableImage;
    TextMeshProUGUI _countText;
    int _count = 1;
    float _elapsedTime;
    public Coroutine BuffCoroutine;
    int Count
    {
        get { return _count; }
        set
        {
            if (_count == value || _data.MaxCount < value)
                return;
            _count = value;
            _countText.text = _count > 1 ? _count.ToString() : "";
        }
    }
    bool _isSetComponents = false;
    void SetComponents()
    {
        if (_isSetComponents)
            return;
        _buffImage = GetComponent<Image>();
        _disableImage = transform.FindChild<Image>(true, name: "DisableImg");
        _countText = transform.FindChild<TextMeshProUGUI>(false);
        _isSetComponents = true;
    }
    public void Init(ItemBuffData data)
    {
        SetComponents();
        _data = data;
        _buffImage.sprite = data.Icon;
        _disableImage.fillAmount = 0f;
        BuffUI.SetUI(data);
    }
    private void OnEnable()
    {
        SetComponents();
        Count = 1;
        _disableImage.fillAmount = 0f;
    }
    private void OnDisable()
    {
        BuffUI.gameObject.SetActive(false);
    }
    public void SetFillAmount(float val)
    {
        _disableImage.fillAmount = val;
    }
    public void CountBuff(PlayerCharacter player)
    {
        if (Count == _data.MaxCount)
            return;
        Count++;
        ItemSetterUtil.ApplyStat(player, _data.Stat1.ItemExtraStatType, _data.Stat1.Value);
        ItemSetterUtil.ApplyStat(player, _data.Stat2.ItemExtraStatType, _data.Stat2.Value);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        BuffUI.gameObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        BuffUI.gameObject.SetActive(false);
    }
    public bool Elapse()
    {
        _elapsedTime += Time.deltaTime;
        float amount = Mathf.Clamp01(_elapsedTime / _data.Duration);
        SetFillAmount(amount);
        if (amount >= 1f)
        {
            var player = PlayerCharacter.Instance;
            gameObject.SetActive(false);
            ItemSetterUtil.RemoveStat(player, _data.Stat1.ItemExtraStatType, _data.Stat1.Value * Count);
            ItemSetterUtil.RemoveStat(player, _data.Stat2.ItemExtraStatType, _data.Stat2.Value * Count);
            return false;
        }
        return true;
    }
    public void ResetTime()
    {
        _elapsedTime = 0f;
    }
    public void OnBuff(PlayerCharacter player, bool last)
    {
        if (last)
            transform.SetAsLastSibling();
        gameObject.SetActive(true);
        if (BuffCoroutine != null)
        {
            ResetTime();
            CountBuff(player);
        }
        else
        {
            ItemSetterUtil.ApplyStat(player, _data.Stat1.ItemExtraStatType, _data.Stat1.Value);
            ItemSetterUtil.ApplyStat(player, _data.Stat2.ItemExtraStatType, _data.Stat2.Value);
            BuffCoroutine = player.StartCoroutine(CoBuff(last));
        }
    }
    IEnumerator CoBuff(bool last)
    {
        while (true)
        {
            if (Elapse() == false)
                break;
            yield return null;
        }
        BuffCoroutine = null;
    }
}
