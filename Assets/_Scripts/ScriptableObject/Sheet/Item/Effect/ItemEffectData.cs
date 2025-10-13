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
public class ItemEffectData
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

        switch (ActionType)
        {
            case ItemActionType.Kill:
                PlayerCharacter.Instance.OnKill += OnEvent;
                break;
            case ItemActionType.UsingSkill:
                PlayerCharacter.Instance.OnUsingSkill += OnEvent;
                break;
            case ItemActionType.UsingAttack:
                PlayerCharacter.Instance.OnUsingAttackStart += OnEvent;
                break;
            case ItemActionType.AttackHit:
                PlayerCharacter.Instance.OnAttackHit += OnEvent;
                break;
            case ItemActionType.StartRound:
                // 맵 매니저 콜백 처리
                break;
            case ItemActionType.DashEnd:
                PlayerCharacter.Instance.OnDashEnd += OnEvent;
                break;
            case ItemActionType.OnSynergy:
                //Manager.Data.ItemsData.Base.TryGetValue(Id, out var data);
                //if (Manager.Item.Synergies[data.SynergyId].Activated == false)
                //    return;
                // 시너지 콜백 처리
                break;
            case ItemActionType.Always:
                // ??
                break;
        }
    }
    float _lastEventTime;
    void OnEvent()
    {
        if (Cooldown > 0f)
        {
            if (Time.time - _lastEventTime < Cooldown)
                return;
            _lastEventTime = Time.time;
        }
        if (Chance != 100f)
        {
            var ran = Random.Range(0f, 100f);
            if (ran > Chance)
                return;
        }
        _eventData.OnEvent(PlayerCharacter.Instance);
    }
}
