using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    List<Vector2> ItemSpawnInfos;
    List<ItemController> _spawnedItems = new List<ItemController>();

    private void Start()
    {
        SpawnItems();
        StartCoroutine(Extension.LateStart(() =>
        {
            Manager.Game.OnMonstersClear += ActiveItems;
            Manager.Item.OnItemAdded += (item) =>
            {
                RemoveItems();
            };
        }));
    }
    void OnDestroy()
    {
        Manager.Game.OnMonstersClear -= ActiveItems;
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
            Gizmos.DrawWireSphere(info, GizmoSize);
        }
    }
#endif

    public void SpawnItems()
    {
        if (ItemSpawnInfos.Count == 0)
            return;
        var list = Manager.Item.MissingItems.ToList();

        var common = list.Where(i => i.ItemGrade == ItemGradeType.Common).ToList();
        var uncommon = list.Where(i => i.ItemGrade == ItemGradeType.Uncommon).ToList();
        var rare = list.Where(i => i.ItemGrade == ItemGradeType.Rare).ToList();
        //var legendary = list.Where(i => i.ItemGrade == ItemGradeType.legendary).ToList();
        List<ItemData> itemsData = new List<ItemData>();
        for (int i = 0; i < 3; i++)
        {
            while (true)
            {
                var type = GetRandomItemGrade(false);
                switch (type)
                {
                    case ItemGradeType.Common:
                        if (common.Count > 0)
                        {
                            var idx = Random.Range(0, common.Count);
                            itemsData.Add(common[idx]);
                            common.RemoveAt(idx);
                            goto Next;
                        }
                        break;
                    case ItemGradeType.Uncommon:
                        if (uncommon.Count > 0)
                        {
                            var idx = Random.Range(0, uncommon.Count);
                            itemsData.Add(uncommon[idx]);
                            uncommon.RemoveAt(idx);
                            goto Next;
                        }
                        break;
                    case ItemGradeType.Rare:
                        if (rare.Count > 0)
                        {
                            var idx = Random.Range(0, rare.Count);
                            itemsData.Add(rare[idx]);
                            rare.RemoveAt(idx);
                            goto Next;
                        }
                        break;
                    case ItemGradeType.Legendary:
                        //if (legendary.Count > 0)
                        break;
                }
            }
        Next:
            continue;
        }

        for (int i = 0; i < ItemSpawnInfos.Count; i++)
        {
            var item = Manager.Item.InstantiateItem(itemsData[i].Id);
            item.transform.parent = transform;
            item.transform.position = ItemSpawnInfos[i];
            item.gameObject.SetActive(false);
            _spawnedItems.Add(item);
            StartCoroutine(Extension.LoadTextureByURL(itemsData[i].IconURL, (texture) =>
            {
                var sprite = texture.ToSprite();
                item.SetSprite(sprite);
            }));
        }
    }
    ItemGradeType GetRandomItemGrade(bool rareAward)
    {
        if (rareAward)
        {
            //var r = Random.Range(0, 100);

            //if (r < 50)
            //    return ItemGradeType.common;
            //else if (r < 80)
            //    return ItemGradeType.uncommon;
            //else if (r < 95)
            //    return ItemGradeType.rare;
            //else
            //    return ItemGradeType.legendary;
        }
        return (ItemGradeType)Random.Range(0, 3);
    }
    void ActiveItems()
    {
        foreach (var item in _spawnedItems)
        {
            item.gameObject.SetActive(true);
        }
    }
    public void RemoveItems()
    {
        Destroy(gameObject);
    }
}
