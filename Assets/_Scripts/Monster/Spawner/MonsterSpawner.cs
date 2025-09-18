using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Collections;

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
    private void Start()
    {
        StartCoroutine(LateStart(() =>
        {
            Manager.Game.MonsterCount += SpawnInfos.Count;
        }));
    }
    IEnumerator LateStart(Action action)
    {
        yield return null;
        action.Invoke();
    }
    public void SpawnAll()
    {
        if (_isSpawned) return;

        foreach (var info in SpawnInfos)
        {
            var monster = Manager.Resource.Instantiate<BaseMonster>(info.Prefab);
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
        SpawnedCount = SpawnInfos.Count;
        _isSpawned = true;
    }
    [SerializeField]
    string key;
    private void Update()
    {
        if (Input.GetKey(key))
        {
            foreach (var m in _spawnedMonsters)
            {
                if (m != null)
                    m.Die();
            }
        }
    }
}
