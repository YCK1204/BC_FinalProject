using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

// 몬스터 스폰 (프리팹, 위치)
// 스폰 조건 (게임 시작 즉시, 다른 스폰 리스트를 다잡고, 플레이어가 특정 범위 내에 접근 시)


[Serializable]
public class MonsterSpawnInfo
{
    public Monster Prefab;
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

    public void SpawnAll()
    {
        if (_isSpawned) return;

        foreach (var info in SpawnInfos)
        {
            var go = Instantiate(info.Prefab);
            if (Root != null) go.transform.parent = Root;
            go.transform.position = info.Position;
            // 몬스터 OnDestroy 시 SpawnedCount 감소
        }
        _isSpawned = true;
    }
}
