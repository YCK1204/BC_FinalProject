using System;
using UnityEngine;

[Serializable]
public struct StatModifier
{
    public float hpModifier;
    public float attackPowerModifier;
    public float scaleModifier;
    public float rangeModifer;

    public StatModifier(float hp = 1f, float attack = 1f, float scale = 1f, float range = 1f)
    {
        hpModifier = hp;
        attackPowerModifier = attack;
        scaleModifier = scale;
        rangeModifer = range;
    }
}

public class MonsterDataHandler : MonoBehaviour
{
    [SerializeField] MonsterData _data;
    public MonsterData Data {  get { return _data; } set { _data = value; } }

    [SerializeField] protected int _curHp;
    public int CurHp { get { return _curHp; } }

    private BaseMonster _owner;
    public BaseMonster Owner { set { _owner = value; } }

    private StatModifier _modifier;

    public float Speed { get { return Data.Speed; } }
    public float AttackPower { get { return Data.AttackPower * _modifier.attackPowerModifier; } }
    public float AttackDelay { get { return Data.AttackDelay; } }
    public float AttackRange { get { return Data.AttackRange * _modifier.rangeModifer; } }
    public float DetectRange { get { return Data.DetectRange * _modifier.rangeModifer; } }
    public bool CanMove { get { return Data.CanMove; } }

    public void Init()
    {
        _curHp = (int)(Data.MaxHp * _modifier.hpModifier);

        if(_owner != null)
        {
            _owner.Sr.transform.localScale = new Vector3(_owner.Sr.transform.localScale.x * _modifier.scaleModifier,
                                                      _owner.Sr.transform.localScale.y * _modifier.scaleModifier,
                                                      _owner.Sr.transform.localScale.z * _modifier.scaleModifier);
            BoxCollider2D col = _owner.Col as BoxCollider2D;
            if(col != null)
                col.size = new Vector2(col.size.x * _modifier.scaleModifier, col.size.y * _modifier.scaleModifier);
        }
    }

    public void SetStatModifier(StatModifier modifier)
    {
        _modifier = modifier;
        Init();
    }

    public void TakeDamage(int damage)
    {
        _curHp -= Mathf.Max(0, damage);
    }

}
