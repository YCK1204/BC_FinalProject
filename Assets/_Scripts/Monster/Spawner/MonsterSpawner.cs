using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

// 몬스터 스폰 (프리팹, 위치)
// 스폰 조건 (게임 시작 즉시, 다른 스폰 리스트를 다잡고, 플레이어가 특정 범위 내에 접근 시)


[Serializable]
public class MonsterSpawnInfo
{
    public BaseMonster Prefab;
    public Vector2 Position;
}
public abstract class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    List<MonsterSpawnInfo> SpawnInfos;
    [SerializeField]
    Transform Root;
    int _spawnedCount = 0;
    [SerializeField]
    List<ChainMonsterSpawner> NextSpawners;
    [SerializeField]
    int FunnelStepNumber = 1;
    bool _isSpawned = false;
    public int SpawnedCount
    {
        get
        {
            return _spawnedCount;
        }
        set
        {
            _spawnedCount = value;
            if (_spawnedCount == 0)
            {
                foreach (var spawner in NextSpawners)
                {
                    spawner.SpawnAll();
                    Object.Destroy(gameObject);
                }
            }
        }
    }
    List<BaseMonster> _spawnedMonsters = new List<BaseMonster>();

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
        foreach (var info in SpawnInfos)
        {
            Gizmos.DrawWireSphere(info.Position, GizmoSize);
        }
    }
#endif
    protected virtual void Init()
    {
        StartCoroutine(LateStart(() =>
        {
            foreach (var info in SpawnInfos)
            {
                if (info.Prefab == null) continue;
                Manager.Game.MonsterCount += info.Prefab.MonsterCount;
            }
        }));
    }
    private void Start()
    {
        Init();
    }
    IEnumerator LateStart(Action action)
    {
        yield return null;
        action.Invoke();
    }
    public void SpawnAll()
    {
        if (_isSpawned) return;

        int numForSort = 1;
        foreach (var info in SpawnInfos)
        {
            if (info.Prefab == null) continue;
            var monster = Manager.Resource.Instantiate<BaseMonster>(info.Prefab);
            SpawnedCount += monster.MonsterCount;
            monster.transform.FindChild<SpriteRenderer>(true).sortingOrder += numForSort;
            monster.OnDied += () =>
            {
                SpawnedCount--;
                Manager.Game.MonsterCount--;
            };
            if (Root != null) monster.transform.parent = Root;
            monster.transform.position = info.Position;
            _spawnedMonsters.Add(monster);
            // 몬스터 OnDestroy 시 SpawnedCount 감소
        }
        _isSpawned = true;
        var step = MapManager.Instance.GetCurrentFunnelStep();
        Manager.Analytics.SendFunnelStep(step, FunnelStepNumber);
    }
}
