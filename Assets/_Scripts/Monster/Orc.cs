using Game.Monster;
using UnityEngine;

public class Orc : Monster, IAttackable, IDamageable, IMovable
{
    public void Attack(IDamageable target)
    {
        if (target == null)
            return;

        target.TakeDamage((int)_attackPower);
    }

    public void Move()
    {

    }

    public void StopMove()
    {
        
    }

    public void TakeDamage(int damage)
    {
        _curHp -= Mathf.Abs(damage);

        if(_curHp <= 0)
        {
            _stateMachine.ChangeState(Common.StateType.Die);
        }
    }
}
