using Game.Monster;
using UnityEngine;

/// <summary>
/// 실제 공격을 실행하는 컴포넌트
/// _attack에 설정된 공격 방식에 따라 공격 방법이 달라짐
/// </summary>
public class MonsterAttack : MonoBehaviour
{
    public float AttackPower { get { return _owner.MonsterData.AttackPower; } }
    public float AttackRange { get { return _owner.MonsterData.AttackRange; } }
    public float AttackDelay { get { return _owner.MonsterData.AttackDelay; } }

    [SerializeField] BaseProjectile _projectile;

    IAttackable _attack;
    public IAttackable Attackable { get { return _attack; } set { _attack = value; _attack?.Init(); } }

    NormalMonster _owner;
    public NormalMonster Owner { get { return _owner; } }

    public System.Action OnAttackEnd;

    public void Init(NormalMonster owner, IAttackable attack = null)
    {
        _owner = owner;
        _attack = attack;
    }

    public void Attack()
    {
        _attack?.Attack();
    }

    public void StopAttack()
    {
        (_attack as BaseAttack)?.StopAttack();
    }

    public void EndAttack()
    {
        StopAttack();
        Invoke("ExcuteAttackEnd", AttackDelay);
    }

    private void ExcuteAttackEnd()
    {
        OnAttackEnd?.Invoke();
    }

    public void CreateProjectile(Transform target = null)
    {
        if (_projectile == null)
            return;

        float dir = Owner.transform.localScale.x;
        BaseProjectile proj = Instantiate(_projectile, transform.position + new Vector3(0.5f * dir,0,0), Quaternion.identity);
        proj.Init(Owner.transform.localScale, _owner.gameObject, target, Owner.MonsterData.AttackPower);
    }
}
