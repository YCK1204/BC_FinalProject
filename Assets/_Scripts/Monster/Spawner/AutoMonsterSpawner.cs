using UnityEngine;

public class AutoMonsterSpawner : MonsterSpawner
{
    protected override void Init()
    {
        base.Init();
        SpawnAll();
    }
}
