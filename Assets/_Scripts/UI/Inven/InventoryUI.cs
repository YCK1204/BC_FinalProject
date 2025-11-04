using Game.Player;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Item Slots")]
    [SerializeField] private Transform _itemSlotParent;
    [SerializeField] private GameObject _itemSlotPrefab;

    [SerializeField] private RectTransform _tooltipPanelRect;
    [SerializeField] private RectTransform _tooltipPanel_msgbox;
    [SerializeField] private Text _itemNameText;
    //[SerializeField] private Text _itemGradeText;
    [SerializeField] private Text _itemStat1Text;
    [SerializeField] private Text _itemStat2Text;
    [SerializeField] private Text _itemEffectText;

    [SerializeField] private GameObject _synergyPanel;
    [SerializeField] private Text _synergyNameText;
    [SerializeField] private Text _synergyText;

    private List<ItemSlotUI> _itemSlots = new List<ItemSlotUI>();
    private Inventory _playerInventory;

    private const float _baseH = 230f;
    private const float _statH = 40f;
    private const float _effH = 50f;

    void Awake()
    {
        InitializeInventory();
        _tooltipPanelRect.gameObject.SetActive(false);
        _synergyPanel.SetActive(false);
    }

    void Start()
    {
        if (PlayerCharacter.Instance != null)
        {
            SetInventory(PlayerCharacter.Instance.Inventory);
            RefreshAllSlots();
        }
    }

    void OnEnable()
    {
        if (PlayerCharacter.Instance != null)
        {
            SetInventory(PlayerCharacter.Instance.Inventory);
            RefreshAllSlots();
        }
    }


    public void SetInventory(Inventory inventory)
    {
        _playerInventory = inventory;
        RefreshAllSlots();
    }

    private void InitializeInventory()
    {
        for (int i = 0; i < 16; i++)
        {
            GameObject slotGO = Instantiate(_itemSlotPrefab, _itemSlotParent);
            ItemSlotUI slotUI = slotGO.GetComponent<ItemSlotUI>();
            if (slotUI != null)
            {
                slotUI.Initialize(this, i);
                _itemSlots.Add(slotUI);
            }
        }
    }


    public void RefreshAllSlots()
    {
        if (_playerInventory == null)
        {
            Debug.LogWarning("인벤설정안됨.");
            return;
        }

        foreach (var slot in _itemSlots)
        {
            slot.ClearSlot();
        }

        int currentSlotIndex = 0;
        foreach (var itemEntry in _playerInventory.Items)
        {
            if (currentSlotIndex < _itemSlots.Count)
            {
                _itemSlots[currentSlotIndex].SetItem(itemEntry.Value);
                currentSlotIndex++;
            }
            else
            {
                break;
            }
        }
    }

    public void ShowTooltip(ItemData itemData)
    {
        _itemNameText.text = itemData.ItemName;
        _itemNameText.color = itemData.TierColor;

        //_itemGradeText.gameObject.SetActive(false);

        float currentTooltipHeight = _baseH;

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
        }

        if (itemData.EffectData != null && !string.IsNullOrEmpty(itemData.EffectText))
        {
            _itemEffectText.text = itemData.EffectText;
            _itemEffectText.gameObject.SetActive(true);
        }
        else
        {
            _itemEffectText.gameObject.SetActive(false);
            currentTooltipHeight -= _effH;
        }

        string synergyName = "";
        string synergyCountString = "";
        int currentSynergyCount = 0;
        const int maxSynergyCount = 3;
        Color activeColor = Color.HSVToRGB(49f / 360f, 34f / 100f, 84f / 100f);
        Color beColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        if (itemData.SynergyId != 0)
        {
            switch (itemData.SynergyId)
            {
                case 1:
                    synergyName = "바람 길";
                    break;
                case 2:
                    synergyName = "섬광";
                    break;
                case 3:
                    synergyName = "날카로움";
                    break;
                default:
                    break;
            }

            if (Manager.Item.Synergies.TryGetValue(itemData.SynergyId, out var synergy))
            {
                currentSynergyCount = synergy.Count;
                synergyCountString = $" ({currentSynergyCount}/{maxSynergyCount})";
            }
        }

        string synergyDescription = itemData.SynergyText;


        if (!string.IsNullOrEmpty(synergyName) || !string.IsNullOrEmpty(synergyDescription))
        {
            _synergyNameText.text = synergyName + synergyCountString;
            _synergyText.text = synergyDescription;

            if (currentSynergyCount >= maxSynergyCount)
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
            synergyPanelRect.anchoredPosition = new Vector2(synergyPanelRect.anchoredPosition.x, -currentTooltipHeight);
        }
        else
        {
            _synergyPanel.gameObject.SetActive(false);
        }

        _tooltipPanelRect.sizeDelta = new Vector2(_tooltipPanelRect.sizeDelta.x, currentTooltipHeight);
        _tooltipPanel_msgbox.sizeDelta = new Vector2(_tooltipPanelRect.sizeDelta.x - 10, currentTooltipHeight - 10);

        _tooltipPanelRect.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        _tooltipPanelRect.gameObject.SetActive(false);
        _synergyPanel.SetActive(false);
    }

    private string FormatStatText(ItemStat stat)
    {
        if (stat.ItemExtraStatType == ItemExtraStatType.None || stat.Value == 0)
        {
            return "";
        }

        string statName = ItemData.ItemExtraStatTypes[stat.ItemExtraStatType];
        string valueString = stat.Value.ToString();

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

    //private string GetItemGradeString(ItemGradeType grade)
    //{
    //    switch (grade)
    //    {
    //        case ItemGradeType.Common: return "일반";
    //        case ItemGradeType.Uncommon: return "고급";
    //        case ItemGradeType.Rare: return "희귀";
    //        case ItemGradeType.Legendary: return "전설";
    //        default: return "";
    //    }
    //}
}