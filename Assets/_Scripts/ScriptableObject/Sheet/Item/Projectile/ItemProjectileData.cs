using Game.Player;
using Google.GData.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public enum ImageType
{
    Animation,
    Sprite
}

[Serializable]
public class ItemProjectileData : ItemAbilityEvent
{
    public int Id;
    public float Duration;
    public float Speed;
    public float Width;
    public float Height;
    public int CollisionCount;
    public float Damage;
    public ImageType ImageType;
    public Sprite sprite;
    public RuntimeAnimatorController Animator;

    public ItemProjectileData(int id, float duration, float speed, float width, float height, int collisionCount, float damage, ImageType imageType, int imageId)
    {
        Id = id;
        Duration = duration;
        Speed = speed;
        Width = width;
        Height = height;
        CollisionCount = collisionCount;
        Damage = damage;
        ImageType = imageType;
        Animator = null;
        sprite = null;

        switch (imageType)
        {
            case ImageType.Animation:
                Animator = Load<RuntimeAnimatorController>($"{imageId}");
                break;
            case ImageType.Sprite:
                sprite = Load<Sprite>($"{imageId}");
                break;
        }
    }
    T Load<T>(string sourceName) where T : UnityEngine.Object
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            if (group.name != "Projectile")
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
        var projectile = Manager.Pool.Pop<ProjectileController>();
        projectile.Init(this, player);
    }
}
