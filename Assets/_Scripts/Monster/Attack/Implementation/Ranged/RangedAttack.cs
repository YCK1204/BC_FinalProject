using Game.Monster;
using UnityEngine;

/// <summary>
/// 원거리 공격 기본 클래스
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
        base.Attack();

        _monsterAttack.CreateProjectile(_target.transform);
    }

    public override bool GetCheckAttackable(float margin = 0)
    {
        float dir = _tr.localScale.x < 0 ? -1 : 1;
        _target = Physics2D.OverlapBox(_tr.position + new Vector3((_attackRange * 0.5f - margin) * dir, 0, 0), new Vector2(_attackRange, 0.5f), 0, _mask);

        return _target != null;
    }
}
