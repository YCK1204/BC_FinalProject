using GameSystem;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    Dictionary<int, ItemData> _items = new Dictionary<int, ItemData>();

    public void AddItem(ItemData item, PlayerCharacter player)
    {
        if (_items.ContainsKey(item.ItemID))
            return;
        _items.Add(item.ItemID, item);
        item.Set(player);
    }
    public void RemoveItem(ItemData item, PlayerCharacter player) 
    {
        if (!_items.ContainsKey(item.ItemID))
            return;
        _items.Remove(item.ItemID);
        item.Unset(player);
    }
    public bool HasItem(int itemID)
    {
        return _items.ContainsKey(itemID);
    }
}
