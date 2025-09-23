using Game.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager
{
    ItemContainer _itemContainer;
    ItemController CurItem;
    Vector2 ContainerOffset = new Vector2(0f, 3f);
    Dictionary<int, ItemData> _items = new Dictionary<int, ItemData>();
    public List<ItemData> MissingItems { get; private set; }
    public void Init()
    {
        var itemContainer = Manager.Resource.Load<ItemContainer>("ItemContainer");

        _itemContainer = Manager.Resource.Instantiate(itemContainer);
        _itemContainer.gameObject.SetActive(false);
        MissingItems = Manager.Data.ItemDict.Values.ToList();
    }
    public void ShowItemInfo(ItemController item)
    {
        CurItem = item;
        _itemContainer.SetUI(CurItem.ItemData);
        _itemContainer.transform.position = (Vector2)item.transform.position + ContainerOffset;
        _itemContainer.gameObject.SetActive(true);
    }
    public void HideItemInfo(ItemController item)
    {
        if (CurItem == item)
        {
            CurItem = null;
            _itemContainer.gameObject.SetActive(false);
        }
    }
    // 아이템 추가/제거, 현재 맵 끝날 시 데이터 저장 필요
    public void AddItem(PlayerCharacter player)
    {
        var item = CurItem;
        var data = item.ItemData;
        if (_items.ContainsKey(data.ItemID))
            return;
        Object.Destroy(CurItem.gameObject);
        _items.Add(data.ItemID, data);
        data.Set(player);
        MissingItems.Remove(data);
    }
    public bool HasItem(int itemID)
    {
        return _items.ContainsKey(itemID);
    }
}
