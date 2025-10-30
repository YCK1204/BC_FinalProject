using UnityEngine;
using UnityEngine.UI;

public class ItemFieldUI : MonoBehaviour
{
    [Header("Field Item Tooltip UI")]
    [SerializeField] private RectTransform _tooltipPanelRect;
    [SerializeField] private RectTransform _tooltipPanel_msgbox;
    [SerializeField] private Text _itemNameText;
    //[SerializeField] private Text _itemGradeText;
    [SerializeField] private Text _itemStat1Text;
    [SerializeField] private Text _itemStat2Text;
    [SerializeField] private Text _itemEffectText;

    [Header("Synergy UI (Part of Field Item Tooltip)")]
    [SerializeField] private GameObject _synergyPanel;
    [SerializeField] private Text _synergyNameText;
    [SerializeField] private Text _synergyText;


    private const float _baseH = 220f;
    private const float _statH = 30f;
    private const float _effH = 30f;
    private const float _baseS = -714f;

    void Awake()
    {
        _tooltipPanelRect.gameObject.SetActive(false);
        _synergyPanel.SetActive(false);
    }

    public void ShowTooltip(ItemData itemData, Vector3 worldPosition = default) // worldPosition은 이제 사용되지 않음
    {
        SetTooltipContent(itemData);

        _tooltipPanelRect.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        _tooltipPanelRect.gameObject.SetActive(false);
        _synergyPanel.SetActive(false);
    }
    private void SetTooltipContent(ItemData itemData)
    {
        _itemNameText.text = itemData.ItemName;
        _itemNameText.color = itemData.TierColor;

        float currentTooltipHeight = _baseH;
        float currentSynergyHeight = _baseS;

        // Stat1
        if (itemData.Stat1 != null && itemData.Stat1.ItemExtraStatType != ItemExtraStatType.None && itemData.Stat1.Value != 0)
        {
            _itemStat1Text.text = FormatStatText(itemData.Stat1);
            _itemStat1Text.gameObject.SetActive(true);
        }
        else
        {
            _itemStat1Text.gameObject.SetActive(false);
            currentTooltipHeight -= _statH;
            currentSynergyHeight += _statH;
        }

        // Stat2
        if (itemData.Stat2 != null && itemData.Stat2.ItemExtraStatType != ItemExtraStatType.None && itemData.Stat2.Value != 0)
        {
            _itemStat2Text.text = FormatStatText(itemData.Stat2);
            _itemStat2Text.gameObject.SetActive(true);
        }
        else
        {
            _itemStat2Text.gameObject.SetActive(false);
            currentTooltipHeight -= _statH;
            currentSynergyHeight += _statH;
        }

        Manager.Data.ItemsData.Effect.TryGetValue(itemData.ItemEffectId, out ItemEffectData effectData);
        if (effectData != null)
        {
            Manager.Data.ItemsData.Description.TryGetValue(effectData.DescId, out var desc);
            _itemEffectText.text = desc.Korean.Replace(',', '\n');
            _itemEffectText.gameObject.SetActive(true);
        }
        else
        {
            _itemEffectText.gameObject.SetActive(false);
            currentTooltipHeight -= _effH;
            currentSynergyHeight += _effH;
        }


        if (itemData.SynergyId != 0 &&
            Manager.Data.ItemsData.Synergy.TryGetValue(itemData.SynergyId, out ItemSynergyData synergyData) &&
            Manager.Item.Synergies.TryGetValue(itemData.SynergyId, out var itemSynergy))
        {
            string synergyName = "";
            switch (itemData.SynergyId)
            {
                case 1: synergyName = "바람 길"; break;
                case 2: synergyName = "섬광"; break;
                case 3: synergyName = "날카로움"; break;
                default: break;
            }
            _synergyNameText.text = $"{synergyName} ({itemSynergy.Count}/{synergyData.Count})";

            Manager.Data.ItemsData.Effect.TryGetValue(synergyData.ItemEffectID, out ItemEffectData synergeEffect);
            if (synergeEffect != null)
            {
                Manager.Data.ItemsData.Description.TryGetValue(synergeEffect.DescId, out var synergeDesc);
                _synergyText.text = $"{synergeDesc.Korean.Replace(',', '\n')}";
            }
            else
            {
                _synergyText.text = "";
            }


            Color activeColor = Color.HSVToRGB(49f / 360f, 34f / 100f, 84f / 100f);
            Color beColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

            if (itemSynergy.Count >= synergyData.Count)
            {
                _synergyNameText.color = activeColor;
                _synergyText.color = activeColor;
            }
            else
            {
                _synergyNameText.color = beColor;
                _synergyText.color = beColor;
            }

            _synergyPanel.gameObject.SetActive(true);

            RectTransform synergyPanelRect = _synergyPanel.GetComponent<RectTransform>();
            synergyPanelRect.anchoredPosition = new Vector2(synergyPanelRect.anchoredPosition.x, currentSynergyHeight);
        }
        else
        {
            _synergyPanel.gameObject.SetActive(false);
        }

        _tooltipPanelRect.sizeDelta = new Vector2(_tooltipPanelRect.sizeDelta.x, currentTooltipHeight);
        _tooltipPanel_msgbox.sizeDelta = new Vector2(_tooltipPanelRect.sizeDelta.x - 10, currentTooltipHeight - 10);
    }

    private string FormatStatText(ItemStat stat)
    {
        if (stat.ItemExtraStatType == ItemExtraStatType.None || stat.Value == 0)
        {
            return "";
        }

        string statName = ItemData.ItemExtraStatTypes[stat.ItemExtraStatType];

        if (stat.ItemExtraStatType == ItemExtraStatType.CriticalChance ||
            stat.ItemExtraStatType == ItemExtraStatType.CriticalDamage ||
            stat.ItemExtraStatType == ItemExtraStatType.AttackSpeed ||
            stat.ItemExtraStatType == ItemExtraStatType.Attack ||
            stat.ItemExtraStatType == ItemExtraStatType.SkillAttack)
        {
            return $"{statName} +{stat.Value}%";
        }
        else
        {
            return $"{statName} +{stat.Value}";
        }
    }
}