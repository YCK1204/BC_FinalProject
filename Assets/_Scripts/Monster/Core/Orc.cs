using Game.Monster;
using UnityEngine;

public class Orc : PatrolMonster, IDamageable
{

    IAttackable _curAttack;

    protected override void Awake()
    {
        base.Awake();

        _attackRange = 1f;

        _curAttack = new MeleeAttack(_attackPower, _attackRange, transform);
        _attack.Init(AttackPower, AttackRange, AttackDelay, _curAttack, this);
    }

    public void TakeDamage(int damage)
    {
        _curHp -= Mathf.Max(0, damage);

        if(_curHp <= 0)
        {
            _stateMachine.ChangeState(Common.StateType.Die);
        }
    }
}
