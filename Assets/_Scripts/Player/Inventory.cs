using Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{
    ItemController _curItem;
    Dictionary<int, ItemData> _items = new Dictionary<int, ItemData>();
    public Action<ItemController> OnItemAdded { get; set; }
    public bool IsDirty { get; private set; } = false;
    public void EnterCurItem(ItemController item)
    {
        _curItem = item;
    }
    public void ExitCurItem(ItemController item)
    {
        if (_curItem == item)
            _curItem = null;
    }
    public void AddItem(PlayerCharacter player)
    {
        if (_curItem == null)
            return;
        var item = _curItem;
        var data = item.ItemData;
        if (_items.ContainsKey(data.Id))
            return;
        IsDirty = true;
        _curItem = null;
        _items.Add(data.Id, data);
        data.Set(player);
        Manager.Item.MissingItems.Remove(data);
        OnItemAdded?.Invoke(item);
        Manager.Item.AddSynergy(data.SynergyId);
    }
    public void Reset()
    {
        _items.Clear();
        Manager.Item.Reset();
    }
    public bool HasItem(int itemID)
    {
        return _items.ContainsKey(itemID);
    }
}
