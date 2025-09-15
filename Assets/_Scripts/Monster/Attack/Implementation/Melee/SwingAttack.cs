using Game.Monster;
using UnityEngine;

public class SwingAttack : MeleeAttack
{
    // Swing 공격은 박스형 탐지 / 반원형 탐지 둘 중 하나 골라야 할 듯
    public SwingAttack(float damage, float attackRange, Transform tr) : base(damage, attackRange, tr)
    {
    }

    /// <summary>
    /// 현재는 일단 반원 모양 탐지로 구현 해봄
    /// </summary>
    public override void Attack()
    {
        float dir = _tr.localScale.x < 0 ? -1 : 1;
        //Collider2D target = Physics2D.OverlapBox(_tr.position + new Vector3(_attackRange * 0.5f * dir, 0, 0), new Vector2(_attackRange, _attackRange), 0, _mask);
        Collider2D target = Physics2D.OverlapCircle(_tr.position, _attackRange, _mask);

        if (target != null)
        {
            if (CheckFov(_tr, target.transform, 180))
                target.GetComponent<IDamageable>()?.TakeDamage((int)_damage);
        }
    }
}
