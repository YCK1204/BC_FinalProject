using Game.Monster;
using Game.Player;
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
        base.Attack();

        if (GetCheckAttackable())
        {
            if(_target != null)
            {
                PlayerCharacter pc = _target.GetComponent<PlayerCharacter>();
                if (pc != null && pc.Invincible)
                    return;

                Vector2 knockBackDir = new Vector2(_monsterAttack.Owner.transform.localScale.x < 0 ? -1 : 1, 0);
                knockBackDir.Normalize();

                // 수치를 어떻게 조정해야하지?
                Rigidbody2D targetRb = _target.GetComponent<Rigidbody2D>();
                targetRb.linearVelocity = Vector2.zero;
                targetRb.AddForce(knockBackDir * 400);
                _target.GetComponent<IDamageable>()?.TakeDamage(_damage, _tr.gameObject);
            }
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
