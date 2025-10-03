using Game.Player;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    ItemAreaData _data;
    Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>(); // 나중에 Area 데이터에 애니메이터 경로 넣고 설정해야함
    }
    public void Init(ItemAreaData data, PlayerCharacter owner)
    {
        _data = data;
        _animator.runtimeAnimatorController = _data.Animator;

        switch (_data.CreatePosType)
        {
            case ItemEffectCreatePosType.Player:
                transform.position = owner.transform.position;
                break;
            case ItemEffectCreatePosType.NearestEnemy:
                transform.position = GetNearestEnemyPosition(owner).position;
                break;
            case ItemEffectCreatePosType.WithInRangeEnemy:
                transform.position = GetInRangeEnemyPosition(owner).position;
                break;
        }
    }
    Transform GetNearestEnemyPosition(PlayerCharacter player)
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _data.Radius, LayerMask.GetMask("Monster"));
        if (hits.Length != 0)
            return hits.OrderBy(x => player.transform.DirectionTo(x.transform)).First().transform;
        return player.transform;
    }
    Transform GetInRangeEnemyPosition(PlayerCharacter player)
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _data.Radius, LayerMask.GetMask("Monster"));
        if (hits.Length != 0)
            return hits[Random.Range(0, hits.Length)].transform;
        return player.transform;
    }
    IEnumerator CoShot()
    {
        float startTime = Time.time;
        while (Time.time - startTime < _data.AnmationDuration)
        {
            yield return null;
        }
        StopCoroutine(CoShot());
        Manager.Pool.Push<AreaController>(gameObject);
    }
    public void HitEnemiesInRadius()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _data.Radius, LayerMask.GetMask("Monster"));

        foreach (var hit in hits)
        {
            var monster = hit.GetComponent<NormalMonster>();
            //monster.TakeDamage((int)_data.Damage);
        }
    }
}
