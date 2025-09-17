using Game.Monster;
using UnityEngine;

public class Orc : PatrolMonster
{
    IAttackable _curAttack;

    protected override void Awake()
    {
        base.Awake();

        _attackRange = 1f;

        _curAttack = new SwingAttack(_attackPower, _attackRange, transform, _attack);
        //_curAttack = new RushAttack(_attackPower, _attackRange, transform, _attack);
        //_curAttack = new RangedAttack(_attackPower, _attackRange, transform, _attack);
        _attack.Init(AttackPower, AttackRange, AttackDelay, _curAttack, this);

        (_curAttack as RushAttack)?.Init();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
    }
}
