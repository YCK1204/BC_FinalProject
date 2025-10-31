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
    DashEnd,
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
        _lastEventTime = 0f;
    }
    ItemAbilityEvent _eventData;
    public void Set()
    {
        Manager.Data.ItemsData.Description.TryGetValue(DescId, out var desc);

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
            case ItemActionType.DashEnd:
                PlayerCharacter.Instance.OnDashEnd += OnEvent;
                break;
            case ItemActionType.Always:
                PlayerCharacter.Instance.OnDied += Clear;
                DisplayManager.Instance.OnEnded += Clear;
                _coAlwaysEffect = Manager.Instance.StartCoroutine(CoAlwaysEffect());
                break;
        }
    }
    void Clear()
    {
        if (_coAlwaysEffect != null)
            Manager.Instance.StopCoroutine(_coAlwaysEffect);
    }
    float _lastEventTime;
    Coroutine _coAlwaysEffect = null;
    IEnumerator CoAlwaysEffect()
    {
        while (true)
        {
            OnEvent();
            yield return new WaitForSeconds(.1f);
        }
    }
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
