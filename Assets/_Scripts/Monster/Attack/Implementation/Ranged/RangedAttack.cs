using Game.Monster;
using UnityEngine;

/// <summary>
/// 원거리 공격
/// 기본적으로 투사체를 발사하는 방식? 추후에 다른 방식도 고려해서 만들 듯
/// </summary>
public class RangedAttack : BaseAttack
{
    public RangedAttack(Transform tr, MonsterAttack monsterAttack) : base(tr, monsterAttack)
    {
    }

    public override void Attack()
    {
        // Todo: 투사체 생성
        // 일단은 일반 생성 -> 이후에 풀에서 가져오는 것으로 변경
        _monsterAttack.CreateProjectile(_target.transform);
    }

    public override bool GetCheckAttackable(float margin = 0)
    {
        float dir = _tr.localScale.x < 0 ? -1 : 1;
        _target = Physics2D.OverlapBox(_tr.position + new Vector3((_attackRange * 0.5f) * dir, 0, 0), new Vector2(_attackRange, 0.5f), 0, _mask);

        return _target != null;
    }
}
