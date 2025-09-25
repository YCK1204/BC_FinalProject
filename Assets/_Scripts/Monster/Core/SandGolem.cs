using Game.Monster;
using UnityEngine;

public class SandGolem : PatrolStateMonster
{
    IAttackable _curAttack;

    protected override void Awake()
    {
        base.Awake();

        _attack.Init(this);
        _curAttack = new RushAttack(transform, _attack);

        _attack.Attackable = _curAttack;
        
        // 슈퍼 아머 영구 적용
        IsSuperArmor = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer == (int)LayerMask.GetMask(Game.Monster.Layers.Player))
        {
            Vector2 knockBackDir = new Vector2(_rb.linearVelocityX < 0 ? -1 : 1 , 1);
            knockBackDir.Normalize();

            // 수치를 어떻게 조정해야하지?
            _target.GetComponent<Rigidbody2D>()?.AddForce(knockBackDir * 200);

            IDamageable damageable = collision.GetComponent<IDamageable>();
            damageable?.TakeDamage((int)_dataHandler.AttackPower);
        }
    }
}
