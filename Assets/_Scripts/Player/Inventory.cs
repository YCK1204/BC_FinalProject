using Game.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{
    ItemController _curItem;
    Dictionary<int, ItemData> _items = new Dictionary<int, ItemData>();
    public Action OnItemAdded { get; set; }
    public bool IsDirty { get; set; } = false;

    public IReadOnlyDictionary<int, ItemData> Items => _items;

    int _gold = 0;
    public int Gold
    {
        get { return _gold; }
        set
        {
            if (_gold != value)
                IsDirty = true;
            _gold = value;
        }
    }
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
        OnItemAdded?.Invoke();
        Manager.Item.AddSynergy(data.SynergyId);
        if (MapManager.Instance != null && MapManager.Instance.CurrentMap.name.IndexOf("base_camp") == -1)
            MapManager.Instance.SetPortal();
        if (MapManager.Instance != null && MapManager.Instance.CurrentMap.name.IndexOf("Stage1") != -1)
            Manager.Analytics.SendFunnelStepForItem(true);
    }
    public void LoadFromJson(InventoryJsonData inventoryJsonData)
    {
        PlayerCharacter player = PlayerCharacter.Instance;
        Gold = inventoryJsonData.Gold;
        var itemsId = inventoryJsonData.ItemsId;
        itemsId.ForEach((id) =>
        {
            Manager.Data.ItemsData.Base.TryGetValue(id, out var data);
            if (data != null)
            {
                _items.Add(id, data);
                data.Set(player);
                Manager.Item.AddSynergy(data.SynergyId);
                Manager.Item.MissingItems.Remove(data);
            }
        });
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
    public string GetItemJsonData()
    {
        var json = new JObject();
        json.Add("gold", Gold);
        var jArray = new JArray();
        _items.Keys.ToList().ForEach((id) => jArray.Add(id));
        json.Add("items_id", jArray);
        return json.ToString();
    }
}
