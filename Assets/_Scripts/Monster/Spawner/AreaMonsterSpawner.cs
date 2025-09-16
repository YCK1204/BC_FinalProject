using UnityEngine;

public class AreaMonsterSpawner : MonsterSpawner
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            SpawnAll();
    }
}
