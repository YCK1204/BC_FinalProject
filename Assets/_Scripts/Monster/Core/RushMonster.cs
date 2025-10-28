using Game.Monster;
using Game.Player;
using UnityEngine;

public class RushMonster : StateMachineMonster
{
    protected override void Awake()
    {
        base.Awake();

        _attack.Init(this);
        _curAttack = new RushAttack(transform, _attack);

        _attack.Attackable = _curAttack;
    }

    protected override void Init()
    {
        base.Init();

        _curPatrolMovement = new PatrolMove(this);
        _curChaseMovement = new ChaseMove(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer == (int)LayerMask.GetMask(Game.Monster.Layers.Player))
        {
            // 무적 체크
            PlayerCharacter pc = collision.GetComponent<PlayerCharacter>();
            if (pc != null && pc.Invincible)
                return;

            Vector2 knockBackDir = new Vector2(_rb.linearVelocityX < 0 ? -1 : 1, 1);
            knockBackDir.Normalize();

            // 수치를 어떻게 조정해야하지?
            Rigidbody2D targetRb = _target?.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.linearVelocity = Vector2.zero;
                targetRb.AddForce(knockBackDir * 300);
            }

            IDamageable damageable = collision.GetComponent<IDamageable>();
            damageable?.TakeDamage(_dataHandler.AttackPower, gameObject);
        }
    }
}
