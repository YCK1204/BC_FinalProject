using Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager
{
    ItemController CurItem;
    Dictionary<int, ItemData> _items = new Dictionary<int, ItemData>();
    public List<ItemData> MissingItems { get; private set; }

    public Action<ItemController> OnItemAdded;
    Dictionary<int, ItemController> _originalItems = new Dictionary<int, ItemController>();
    public void Init()
    {
        MissingItems = Manager.Data.ItemsData.Base.Values.ToList();

        var proj = Manager.Resource.Load<ProjectileController>("Projectile");
        Manager.Pool.CreatePool<ProjectileController>(10, proj.gameObject);
        var item = Manager.Resource.Load<ItemController>("ItemController");
        GameObject go = new GameObject("ItemRoot");
        GameObject.DontDestroyOnLoad(go);
        foreach (var data in MissingItems)
        {
            var itemInstance = GameObject.Instantiate(item, go.transform);
            itemInstance.gameObject.SetActive(false);
            itemInstance.SetData(data);
            _originalItems.Add(data.Id, itemInstance);
        }
    }
    public ItemController InstantiateItem(int id)
    {
        var item = GameObject.Instantiate(_originalItems[id]);
        item.SetData(_originalItems[id].ItemData);
        return item;
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
