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
    List<ItemSpawnInfo> ItemSpawnInfos;

    bool _isSpawned = false;

    private void Start()
    {
        StartCoroutine(Extension.LateStart(() =>
        {
            Manager.Game.OnMonstersClear += SpawnItems;
        }));
    }
#if UNITY_EDITOR
    [SerializeField]
    Color GizmoColor = Color.white;
    [SerializeField, Range(1f, 3f)]
    float GizmoSize = 1f;
    void OnValidate()
    {
        UnityEditor.SceneView.RepaintAll();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        foreach (var info in ItemSpawnInfos)
        {
            Gizmos.DrawWireSphere(info.Position, GizmoSize);
        }
    }
#endif

    public void SpawnItems()
    {
        if (_isSpawned)
            return;
        foreach (var info in ItemSpawnInfos)
        {
            var item = Manager.Resource.Instantiate(ItemPrefab);
            item.transform.SetParent(transform);
            item.transform.position = info.Position;
            item.SetData(info.ItemData);
        }
        _isSpawned = true;
    }
}
