using Game.Monster;
using UnityEngine;

public class Orc : PatrolMonster
{
    IAttackable _curAttack;

    protected override void Awake()
    {
        base.Awake();

        _attack.Init(_dataHandler.AttackPower, _dataHandler.AttackRange, _dataHandler.AttackDelay, this);
        _curAttack = new SwingAttack(_dataHandler.AttackPower, _dataHandler.AttackRange, transform, _attack);
        //_curAttack = new RushAttack(_dataHandler.AttackPower, _attackRange, transform, _attack);
        //_curAttack = new RangedAttack(_attackPower, _attackRange, transform, _attack);

        _attack.Attackable = _curAttack;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
    }
}
