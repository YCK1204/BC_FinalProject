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

    [Header("Tooltip UI")]
    [SerializeField] private GameObject _tooltipPanel;
    [SerializeField] private Text _itemNameText;
    [SerializeField] private Text _itemGradeText;
    [SerializeField] private Text _itemStat1Text;
    [SerializeField] private Text _itemStat2Text;
    [SerializeField] private Text _itemEffectText;

    private List<ItemSlotUI> _itemSlots = new List<ItemSlotUI>();
    private Inventory _playerInventory;

    private const int INVENTORY_SIZE = 16;

    void Awake()
    {
        InitializeInventory();
        _tooltipPanel.SetActive(false);
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
        RefreshAllSlots();
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
        _itemGradeText.text = GetItemGradeString(itemData.ItemGrade);
        _itemGradeText.color = itemData.TierColor;

        _itemStat1Text.text = FormatStatText(itemData.Stat1);
        _itemStat2Text.text = FormatStatText(itemData.Stat2);

        //if (itemData.EffectData != null && !string.IsNullOrEmpty(itemData.EffectData.Description))
        //{
        //    _itemEffectText.text = itemData.EffectData.Description;
        //    _itemEffectText.gameObject.SetActive(true);
        //}
        //else
        //{
        //    _itemEffectText.gameObject.SetActive(false);
        //}

        _tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        _tooltipPanel.SetActive(false);
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
            stat.ItemExtraStatType == ItemExtraStatType.SkillHaste ||
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

    private string GetItemGradeString(ItemGradeType grade)
    {
        switch (grade)
        {
            case ItemGradeType.Common: return "일반";
            case ItemGradeType.Uncommon: return "고급";
            case ItemGradeType.Rare: return "희귀";
            case ItemGradeType.Legendary: return "전설";
            default: return "";
        }
    }
}