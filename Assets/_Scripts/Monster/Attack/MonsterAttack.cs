using Game.Monster;
using UnityEngine;

/// <summary>
/// 실제 공격을 실행하는 컴포넌트
/// _attack에 설정된 공격 방식에 따라 공격 방법이 달라짐
/// </summary>
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
