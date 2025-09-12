using Game.Monster;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    float _attackPower;
    float _attackRange;
    float _attackSpeed;

    IAttackable _attack;

    Monster _owner;

    public System.Action OnAttackEnd;

    public void Init(float attackPower, float attackRange, float attackSpeed, IAttackable attack, Monster owner)
    {
        _attackPower = attackPower;
        _attackRange = attackRange;
        _attackSpeed = attackSpeed;
        _attack = attack;
        _owner = owner;
    }

    public void Attack()
    {
        _attack?.Attack();
    }

    public void StopAttack()
    {
        Invoke("ExcuteAttackEnd", _attackSpeed);
    }

    private void ExcuteAttackEnd()
    {
        OnAttackEnd?.Invoke();
    }
}
