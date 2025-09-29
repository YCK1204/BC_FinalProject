using Game.Player;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct ItemBuffData : ItemAbilityEvent
{
    public int Id;
    public float Duration;
    public int MaxCount;
    public ItemExtraStatType Ability1;
    public float Ability1Value;
    public ItemExtraStatType Ability2;
    public float Ability2Value;
    public string IconURL;
    public string Name;
    public int DescriptionId;
    public ItemBuffData(int id, float duration, int maxCount, ItemExtraStatType ability1, float ability1Value, ItemExtraStatType ability2, float ability2Value, string iconURL, string name, int descriptionId)
    {
        Id = id;
        Duration = duration;
        MaxCount = maxCount;
        Ability1 = ability1;
        Ability1Value = ability1Value;
        Ability2 = ability2;
        Ability2Value = ability2Value;
        IconURL = iconURL;
        Name = name;
        DescriptionId = descriptionId;
        _lastBuffTime = 0f;
        _buffCoroutine = null;
    }
    float _lastBuffTime;
    Coroutine _buffCoroutine;
    public void OnEvent(PlayerCharacter player)
    {
        _lastBuffTime = Time.time;
        if (_buffCoroutine == null)
            _buffCoroutine = player.StartCoroutine(CoBuff(player));
    }
    IEnumerator CoBuff(PlayerCharacter player)
    {
        ItemSetterUtil.ApplyStat(player, Ability1, Ability1Value);
        ItemSetterUtil.ApplyStat(player, Ability2, Ability2Value);
        while (Time.time - _lastBuffTime < Duration)
        {
            yield return null;
        }
        ItemSetterUtil.RemoveStat(player, Ability1, Ability1Value);
        ItemSetterUtil.RemoveStat(player, Ability2, Ability2Value);
        _buffCoroutine = null;
    }
}
