using Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public enum ConditionType
{
    Always,
    Normal,
    Awaken,
}
public enum AbilityType
{
    Area,
    Buff,
    Projectile,
}

public enum ItemActionType
{
    Always,
    Kill,
    UsingSkill,
    UsingAttack,
    AttackHit,
    StartRound,
    DashEnd,
    OnSynergy,
}

[Serializable]
public struct ItemEffectData
{
    public int Id;
    public ConditionType Condition;
    public string Nmae;
    public ItemActionType ActionType;
    public float Chance;
    public int SpecialAbilityId;
    public AbilityType AbilityType;
    public float Cooldown;
    public int DescId;
    public string Desc;
    public ItemEffectData(int id, ConditionType condition, string name, ItemActionType actionType, float chance, int specialAbilityId, AbilityType abilityType, float cooldown, int descId)
    {
        Id = id;
        Condition = condition;
        Nmae = name;
        ActionType = actionType;
        Chance = chance;
        SpecialAbilityId = specialAbilityId;
        AbilityType = abilityType;
        Cooldown = cooldown;
        DescId = descId;
        Desc = "";
        _eventData = default(ItemAbilityEvent);
        _lastEventTime = 0f;
    }
    ItemAbilityEvent _eventData;
    public void Set()
    {
        Manager.Data.ItemsData.Description.TryGetValue(DescId, out var desc);
        Desc = desc.Korean;

        Dictionary<int, ItemAbilityEvent> dict = new Dictionary<int, ItemAbilityEvent>();
        switch (AbilityType)
        {
            case AbilityType.Area:
                dict = Manager.Data.ItemsData.Area.ToDictionary(dict => dict.Key, dict => (ItemAbilityEvent)dict.Value);
                break;
            case AbilityType.Buff:
                dict = Manager.Data.ItemsData.Buff.ToDictionary(dict => dict.Key, dict => (ItemAbilityEvent)dict.Value);
                break;
            case AbilityType.Projectile:
                dict = Manager.Data.ItemsData.Projectile.ToDictionary(dict => dict.Key, dict => (ItemAbilityEvent)dict.Value);
                break;
        }
        _eventData = dict[SpecialAbilityId];

        //switch (ActionType)
        //{
        //    case ItemActionType.Kill:
        //        PlayerCharacter.Instance.OnKillEnemy -= OnEvent;
        //        PlayerCharacter.Instance.OnKillEnemy += OnEvent;
        //        break;
        //    case ItemActionType.UsingSkill:
        //        PlayerCharacter.Instance.Input.Skill.performed -= ctx => OnEvent();
        //        PlayerCharacter.Instance.Input.Skill.performed += ctx => OnEvent();
        //        break;
        //    case ItemActionType.UsingAttack:
        //        PlayerCharacter.Instance.Input.Attack.performed -= ctx => OnEvent();
        //        break;
        //    case ItemActionType.AttackHit:
        //        PlayerCharacter.Instance.OnAttackHit -= OnEvent;
        //        PlayerCharacter.Instance.OnAttackHit += OnEvent;
        //        break;
        //    case ItemActionType.StartRound:
        //        break;
        //    case ItemActionType.DashEnd:
        //        break;
        //    case ItemActionType.OnSynergy:
        //        break;
        //        PlayerCharacter.Instance.Input.Dash.canceled -= ctx => OnEvent();
        //}
        //PlayerCharacter.Instance
    }
    float _lastEventTime;
    void OnEvent()
    {
        var ran = Random.Range(0f, 100f);
        if (ran > Chance)
            return;
        if (Time.time - _lastEventTime < Cooldown)
            return;
        _lastEventTime = Time.time;
        _eventData.OnEvent(PlayerCharacter.Instance);
    }
}
