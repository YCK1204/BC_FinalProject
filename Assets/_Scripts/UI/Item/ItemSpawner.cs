using Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            PlayerCharacter.Instance.Inventory.OnItemAdded += (item) =>
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
        var legendary = list.Where(i => i.ItemGrade == ItemGradeType.Legendary).ToList();
        List<ItemData> itemsData = new List<ItemData>();
        for (int i = 0; i < 3; i++)
        {
            while (true)
            {
                var type = GetRandomItemGrade();
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
                        if(legendary.Count > 0)
                        {
                            var idx = Random.Range(0, legendary.Count);
                            itemsData.Add(legendary[idx]);
                            legendary.RemoveAt(idx);
                            goto Next;
                        }
                        break;
                }
            }
        Next:
            continue;
        }

        for (int i = 0; i < ItemSpawnInfos.Count; i++)
        {
            var item = Manager.Item.InstantiateItem(itemsData[i].Id);
            item.transform.position = ItemSpawnInfos[i];
            item.gameObject.SetActive(false);
            _spawnedItems.Add(item);
            Manager.Item.ItemIcons.TryGetValue(itemsData[i].Id, out var sprite);
            item.SetSprite(sprite);
        }
    }
    ItemGradeType GetRandomItemGrade()
    {
        var r = Random.Range(0f, 100f);

        if (r < 40f)
            return ItemGradeType.Common;
        else if (r < 70f)
            return ItemGradeType.Uncommon;
        else if (r < 90f)
            return ItemGradeType.Rare;
        else
            return ItemGradeType.Legendary;
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
        PlayerCharacter.Instance.Inventory.OnItemAdded -= (item) =>
        {
            RemoveItems();
        };
        foreach (var item in _spawnedItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
    }
}
