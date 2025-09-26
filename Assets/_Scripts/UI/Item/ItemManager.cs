using Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager
{
    ItemController CurItem;
    Vector2 ContainerOffset = new Vector2(0f, 3f);
    Dictionary<int, ItemData> _items = new Dictionary<int, ItemData>();
    public List<ItemData> MissingItems { get; private set; }

    public Action<ItemController> OnItemAdded;
    public void Init()
    {
        MissingItems = Manager.Data.ItemDict.Values.ToList();
    }
    public void OnTriggerEnterItem(ItemController item)
    {
        CurItem = item;
    }
    public void OnTriggerExitItem(ItemController item)
    {
        CurItem = null;
    }
    // 아이템 추가/제거, 현재 맵 끝날 시 데이터 저장 필요
    public void AddItem(PlayerCharacter player)
    {
        if (CurItem == null)
            return;
        var item = CurItem;
        var data = item.ItemData;
        if (_items.ContainsKey(data.Id))
            return;
        _items.Add(data.Id, data);
        data.Set(player);
        MissingItems.Remove(data);
        OnItemAdded?.Invoke(item);
    }
    public bool HasItem(int itemID)
    {
        return _items.ContainsKey(itemID);
    }
}
