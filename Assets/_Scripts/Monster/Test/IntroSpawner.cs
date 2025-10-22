using System.Collections.Generic;
using UnityEngine;

public class IntroSpawner : MonoBehaviour
{
    public GameObject TutorialMonsterPrefab;
    public GameObject Stopper;

    public List<Vector2> SpawnPosition;

    private int _killCount;
    private bool _isSpawned;

    private void Awake()
    {
        _isSpawned = false;
        _killCount = SpawnPosition.Count;
        Stopper.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !_isSpawned)
        {
            _isSpawned = true;
            SpawnTutorialSlime();
        }
    }

    private void SpawnTutorialSlime()
    {
        foreach(Vector2 pos in SpawnPosition)
        {
            Instantiate(TutorialMonsterPrefab, pos, Quaternion.identity).GetComponent<BaseMonster>().OnDied += MonsterDie;
        }
    }

    private void MonsterDie()
    {
        _killCount--;

        if( _killCount <= 0 )
        {
            Stopper.SetActive(false);
            Manager.Analytics.SendFunnelStep(FunnelStep.None, 7);
        }
    }

    private void OnDrawGizmos()
    {
        if (SpawnPosition == null || SpawnPosition.Count == 0)
            return;

        Gizmos.color = Color.red;
        foreach(Vector2 pos in SpawnPosition)
        {
            Gizmos.DrawWireSphere(pos, 1f);
        }
    }
}
