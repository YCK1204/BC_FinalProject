using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private int _slotIndex;

    private ItemData _currentItemData;
    private InventoryUI _inventoryUI;

    public int SlotIndex => _slotIndex;

    public void Initialize(InventoryUI inventoryUI, int index)
    {
        _inventoryUI = inventoryUI;
        _slotIndex = index;
        ClearSlot();
    }

    public void SetItem(ItemData itemData)
    {
        _currentItemData = itemData;
        if (_itemIcon != null && itemData != null && Manager.Item.ItemIcons.TryGetValue(itemData.Id, out Sprite cachedSprite))
        {
            _itemIcon.sprite = cachedSprite;
            _itemIcon.gameObject.SetActive(true);
        }
        else
        {
            _itemIcon.gameObject.SetActive(false);
        }
    }

    public void ClearSlot()
    {
        _currentItemData = null;
        if (_itemIcon != null)
        {
            _itemIcon.sprite = null;
            _itemIcon.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItemData != null)
        {
            _inventoryUI.ShowTooltip(_currentItemData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inventoryUI.HideTooltip();
    }
}