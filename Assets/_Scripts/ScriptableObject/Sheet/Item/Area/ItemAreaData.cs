using Game.Player;
using System;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using Random = UnityEngine.Random;
public enum ItemEffectCreatePosType
{
    Player,
    NearestEnemy,
    WithInRangeEnemy,
}

[Serializable]
public class ItemAreaData : ItemAbilityEvent
{
    public int Id;
    public ItemEffectCreatePosType CreatePosType;
    public float Radius;
    public float Damage;
    public RuntimeAnimatorController Animator;

    public ItemAreaData(int id, ItemEffectCreatePosType createPosType, float radius, float damage, int animId)
    {
        Id = id;
        CreatePosType = createPosType;
        Radius = radius;
        Damage = damage;
        Animator = Load<RuntimeAnimatorController>($"{animId}");
    }
    T Load<T>(string sourceName) where T : UnityEngine.Object
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            if (group.name != "Area")
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
    public void OnEvent(PlayerCharacter player)
    {
        var area = Manager.Pool.Pop<AreaController>();
        area.Init(this, player);
    }
}
