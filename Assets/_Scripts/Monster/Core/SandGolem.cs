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
        if (collision.gameObject.layer == (int)LayerMask.GetMask(Game.Monster.Layers.Player))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            damageable?.TakeDamage((int)_dataHandler.AttackPower);
        }
    }
}
