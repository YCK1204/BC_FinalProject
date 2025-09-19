using Game.Monster;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SwingAttack : MeleeAttack
{
    // Swing 공격은 박스형 탐지 / 반원형 탐지 둘 중 하나 골라야 할 듯
    public SwingAttack(Transform tr, MonsterAttack monsterAttack) : base(tr, monsterAttack)
    {
    }

    /// <summary>
    /// 현재는 일단 사각형으로 공격 범위 구현
    /// </summary>
    public override void Attack()
    {
        if(GetCheckAttackable())
        {
            Debug.Log($"{_tr.name}의 공격력: {_damage}");
            if(_target != null)
                _target.GetComponent<IDamageable>()?.TakeDamage((int)_damage);
        }
    }

    public override bool GetCheckAttackable(float margin = 0)
    {
        //_target = Physics2D.OverlapCircle(_tr.position, _attackRange, _mask);
        float dir = _tr.localScale.x < 0 ? -1 : 1; 
        _target = Physics2D.OverlapBox(_tr.position + new Vector3((_attackRange * 0.5f - margin) * dir, 0, 0), new Vector2(_attackRange, _attackRange), 0, _mask);
        if (_target != null)
        {
            return CheckFov(_tr, _target.transform, 180);
        }
        return false;
    }
}
