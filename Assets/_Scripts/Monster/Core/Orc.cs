using Game.Monster;
using UnityEngine;

public class Orc : StateMachineMonster
{
    protected override void Awake()
    {
        base.Awake();

        _attack.Init(this);
        _curAttack = new SwingAttack(transform, _attack);
        //_curAttack = new RushAttack(transform, _attack);
        //_curAttack = new RangedAttack( transform, _attack);

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
        if(collision.gameObject.layer == (int)LayerMask.GetMask(Game.Monster.Layers.Player))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            damageable?.TakeDamage((int)_dataHandler.AttackPower);
        }
    }
}
