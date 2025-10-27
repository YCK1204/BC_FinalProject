using Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager
{
    public BuffManager Buff = new BuffManager();
    public List<ItemData> MissingItems;
    public Dictionary<int, ItemSynergy> Synergies = new Dictionary<int, ItemSynergy>();
    Dictionary<int, ItemController> _originalItems = new Dictionary<int, ItemController>();
    public Dictionary<int, Sprite> ItemIcons = new Dictionary<int, Sprite>();
    public void Reset()
    {
        MissingItems = Manager.Data.ItemsData.Base.Values.ToList();
        Synergies = Manager.Data.ItemsData.Synergy.Values.ToDictionary(x => x.Id, x => new ItemSynergy(x));
    }
    public void Init()
    {
        Buff.Init();
        Reset();
        CreateItemPool();
        SetItems();
    }
    async void SetItems()
    {
        var item = Manager.Resource.Load<ItemController>("ItemController");
        GameObject go = new GameObject("ItemRoot");
        GameObject.DontDestroyOnLoad(go);
        foreach (var data in MissingItems)
        {
            var itemInstance = GameObject.Instantiate(item, go.transform);
            itemInstance.gameObject.SetActive(false);
            itemInstance.SetData(data);
            _originalItems.Add(data.Id, itemInstance);
            var texture = await Extension.LoadTextureByURLAsync(data.IconURL);
            if (texture != null)
                ItemIcons.Add(data.Id, texture.ToSprite());
        }
    }
    void CreateItemPool()
    {
        Manager.Resource.LoadAsync("ItemEffectObject", (list) =>
        {
            foreach (var item in list)
            {
                switch (item.name)
                {
                    case "Projectile":
                        Manager.Pool.CreatePool<ProjectileController>(10, item.GetComponent<ProjectileController>().gameObject);
                        break;
                    case "Area":
                        Manager.Pool.CreatePool<AreaController>(10, item.GetComponent<AreaController>().gameObject);
                        break;
                }
            }
        });
    }
    public ItemController InstantiateItem(int id)
    {
        var item = GameObject.Instantiate(_originalItems[id]);
        item.SetData(_originalItems[id].ItemData);
        return item;
    }
    public void AddSynergy(int synergyID)
    {
        if (Synergies.TryGetValue(synergyID, out var synergy))
            synergy.Count++;
    }

    public void Clear()
    {
        Buff.Release();
    }
}
