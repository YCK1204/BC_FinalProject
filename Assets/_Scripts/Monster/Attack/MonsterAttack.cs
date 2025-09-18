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

    [SerializeField] BaseProjectile _projectile;

    IAttackable _attack;
    public IAttackable Attackable { get { return _attack; } set { _attack = value; } }

    BaseMonster _owner;
    public BaseMonster Owner { get { return _owner; } }

    public System.Action OnAttackEnd;

    public void Init(float attackPower, float attackRange, float attackSpeed, BaseMonster owner, IAttackable attack = null)
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

    public void CreateProjectile(Transform target = null)
    {
        if (_projectile == null)
            return;

        float dir = Owner.transform.localScale.x > 0 ? 1f : -1f;
        BaseProjectile proj = Instantiate(_projectile, transform.position + new Vector3(0.5f * dir,0,0), Quaternion.identity);
        proj.Init(Owner.transform.localScale, target);
    }
}
