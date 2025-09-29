using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Cinemachine.CinemachineFreeLookModifier;

[Serializable]
public class StatModifier
{
    Game.Monster.StatType _targetStat;
    Game.Monster.ModifierType _modifierType;
    float _value;
    BaseMonster _caster;

    public Game.Monster.StatType TargetStat { get { return _targetStat; } }
    public Game.Monster.ModifierType ModifierType { get { return _modifierType; } }
    public float Value { get { return _value; } }
    public BaseMonster Caster { get { return _caster; } }

    public StatModifier(Game.Monster.StatType stat, Game.Monster.ModifierType modifier, float value, BaseMonster caster)
    {
        _targetStat = stat;
        _modifierType = modifier;
        _value = value;
        _caster = caster;
    }
}

public class MonsterDataHandler : MonoBehaviour
{
    [SerializeField] MonsterData _data;
    public MonsterData Data {  get { return _data; } set { _data = value; } }

    [SerializeField] protected float _curHp;
    public float CurHp { get { return _curHp; } }

    private BaseMonster _owner;
    public BaseMonster Owner { set { _owner = value; } }

    private List<StatModifier> _modifierList;

    private void Awake()
    {
        _modifierList = new List<StatModifier>();
    }

    public float Speed
    {
        get
        {
            float add, mul;
            GetStat(Game.Monster.StatType.Speed, out add, out mul);
            return (Data.Speed + add) * mul;
        }
    }
    public float AttackPower
    {
        get
        {
            float add, mul;
            GetStat(Game.Monster.StatType.Attack, out add, out mul);
            return (Data.AttackPower + add) * mul;
        }
    }
    public float AttackDelay { get { return Data.AttackDelay; } }
    public float AttackRange
    {
        get
        {
            float add, mul;
            GetStat(Game.Monster.StatType.Scale, out add, out mul);
            return (Data.AttackRange + add) * mul;
        }
    }
    public float DetectRange
    {
        get
        {
            float add, mul;
            GetStat(Game.Monster.StatType.Scale, out add, out mul);
            return (Data.DetectRange + add) * mul;
        }
    }
    public float MaxHp
    {
        get
        {
            float add, mul;
            GetStat(Game.Monster.StatType.Hp, out add, out mul);
            return (Data.MaxHp + add) * mul;
        }
    }
    public bool CanMove { get { return Data.CanMove; } }

    public void Init()
    {
        // 능력치 버프 리스트가 비어있으면 새로 생성
        if(_modifierList == null)
        {
            _modifierList = new List<StatModifier>();
        }

        _curHp = MaxHp;

        // 몬스터 실제 크기 변환
        if(_owner != null)
        {
            float add, mul;
            GetStat(Game.Monster.StatType.Scale, out add, out mul);

            if(_owner is NormalMonster)
            {
                NormalMonster owner = (NormalMonster)_owner;

                owner.transform.localScale = new Vector3(owner.transform.localScale.x * mul,
                                                          owner.transform.localScale.y * mul,
                                                          owner.transform.localScale.z * mul);
                BoxCollider2D col = owner.Col as BoxCollider2D;

                if (col != null)
                    col.size = new Vector2(col.size.x * mul, col.size.y * mul);
            }
        }
    }

    public void AddModifier(StatModifier modifier)
    {
        _modifierList.Add(modifier);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        _modifierList.Remove(modifier);
    }

    public void RemoveModifierByCaster(NormalMonster caster)
    {
        for(int i=0; i<_modifierList.Count;i++)
        {
            if (_modifierList[i].Caster == caster)
            {
                RemoveModifier(_modifierList[i]);
                i--;
            }
        }
    }

    public void GetStat(Game.Monster.StatType stat, out float add, out float mul)
    {
        add = 0f;
        mul = 1f;

        if(_modifierList == null)
            _modifierList = new List<StatModifier>();

        foreach(StatModifier modifier in _modifierList)
        {
            if(modifier.TargetStat == stat)
            {
                switch(modifier.ModifierType)
                {
                    case Game.Monster.ModifierType.Add:
                        add += modifier.Value;
                        break;
                    case Game.Monster.ModifierType.Multiply:
                        mul *= modifier.Value;
                        break;
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _curHp -= Mathf.Max(0, damage);
    }

}
