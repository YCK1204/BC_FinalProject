using Game.Monster;
using Game.Player;
using UnityEngine;

public class StabAttack : MeleeAttack
{
    public StabAttack(Transform tr, MonsterAttack monsterAttack) : base(tr, monsterAttack)
    {
    }

    public override void Attack()
    {
        base.Attack();

        if (GetCheckAttackable())
        {
            PlayerCharacter pc = _target.GetComponent<PlayerCharacter>();
            if (pc != null && pc.Invincible)
                return;

            Vector2 knockBackDir = new Vector2(_monsterAttack.Owner.transform.localScale.x < 0 ? -1 : 1, 0);
            knockBackDir.Normalize();

            Rigidbody2D targetRb = _target.GetComponent<Rigidbody2D>();
            targetRb.linearVelocity = Vector2.zero;
            targetRb.AddForce(knockBackDir * 400);
            _target.GetComponent<IDamageable>()?.TakeDamage(_damage, _tr.gameObject);
        }
    }

    public override bool GetCheckAttackable(float margin = 0)
    {
        float dir = _tr.localScale.x < 0 ? -1 : 1;
        _target = Physics2D.OverlapBox(_tr.position + new Vector3((_attackRange * 0.5f - margin) * dir, 0, 0), new Vector2(_attackRange, 0.5f), 0, _mask);

        return _target != null;
    }
}
