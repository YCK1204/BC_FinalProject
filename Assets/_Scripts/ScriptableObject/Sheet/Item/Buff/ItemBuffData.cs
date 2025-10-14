using Game.Player;
using System;
using System.Collections;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

[Serializable]
public class ItemBuffData : ItemAbilityEvent
{
    public int Id;
    public float Duration;
    public int MaxCount;
    public ItemStat Stat1;
    public ItemStat Stat2;
    public string IconURL;
    public string Name;
    public int DescriptionId;
    public Sprite Icon;
    public ItemBuffData(int id, float duration, int maxCount, ItemExtraStatType ability1, float ability1Value, ItemExtraStatType ability2, float ability2Value, string iconURL, string name, int descriptionId, int imageId)
    {
        Id = id;
        Duration = duration;
        MaxCount = maxCount;
        Stat1 = new ItemStat() { ItemExtraStatType = ability1, ItemTriggerType = ItemTriggerType.Always, Value = ability1Value };
        Stat2 = new ItemStat() { ItemExtraStatType = ability2, ItemTriggerType = ItemTriggerType.Always, Value = ability2Value };
        IconURL = iconURL;
        Name = name;
        DescriptionId = descriptionId;
        _lastBuffTime = 0f;
        _buffCoroutine = null;
        Icon = Extension.LoadWithAddresssableByGroup<Sprite>($"{imageId}", "Buff");
    }
    float _lastBuffTime;
    Coroutine _buffCoroutine;
    public void OnEvent(PlayerCharacter player)
    {
        Manager.Item.Buff.OnBuff(this, player);
    }
}
