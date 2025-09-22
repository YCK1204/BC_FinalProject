using Game.Monster;
using UnityEngine;

public class Orc : PatrolStateMonster
{
    IAttackable _curAttack;

    protected override void Awake()
    {
        base.Awake();

        _attack.Init(this);
        //_curAttack = new SwingAttack(transform, _attack);
        _curAttack = new RushAttack(transform, _attack);
        //_curAttack = new RangedAttack(_attackPower, _attackRange, transform, _attack);

        _attack.Attackable = _curAttack;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == (int)LayerMask.GetMask(Game.Monster.Layers.Player))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            damageable?.TakeDamage((int)_dataHandler.AttackPower);
        }
    }
}
