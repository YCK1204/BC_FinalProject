using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSpawnInfo
{
    public ItemData ItemData;
    public Vector2 Position;
}
public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    ItemController ItemPrefab;
    [SerializeField]
    ItemContainer ItemContainerPrefab;
    [SerializeField]
    List<ItemSpawnInfo> ItemSpawnInfos;

    bool _isSpawned = false;

    private void Start()
    {
        SpawnItems();
    }
    public void SpawnItems()
    {
        if (_isSpawned)
            return;
        foreach (var info in ItemSpawnInfos)
        {
            var item = Manager.Resource.Instantiate(ItemPrefab);
            item.transform.SetParent(transform);
            item.transform.position = info.Position;
            item.SetData(info.ItemData, ItemContainerPrefab);
        }
        _isSpawned = true;
    }
}
