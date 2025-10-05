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
    public ItemExtraStatType Ability1;
    public float Ability1Value;
    public ItemExtraStatType Ability2;
    public float Ability2Value;
    public string IconURL;
    public string Name;
    public int DescriptionId;
    public Sprite Icon;
    public ItemBuffData(int id, float duration, int maxCount, ItemExtraStatType ability1, float ability1Value, ItemExtraStatType ability2, float ability2Value, string iconURL, string name, int descriptionId, int imageId)
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
        Icon = Load<Sprite>(imageId.ToString());
    }
    T Load<T>(string sourceName) where T : UnityEngine.Object
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            if (group.name != "Buff")
                continue;
            foreach (var entry in group.entries)
            {
                if (entry.address == sourceName)
                {
                    string path = AssetDatabase.GUIDToAssetPath(entry.guid);
                    return (AssetDatabase.LoadAssetAtPath<T>(path));
                }
            }
        }
        return null;
    }
    float _lastBuffTime;
    Coroutine _buffCoroutine;
    public void OnEvent(PlayerCharacter player)
    {
        Manager.Item.Buff.OnBuff(this, player);
    }
}
