using Game.Player;
using System;
using UnityEngine;

public enum ImageType
{
    Animation,
    Sprite
}

public enum ProjectileMoveType
{
    Forward,
    Tracking,
    Oblque,
}

[Serializable]
public class ItemProjectileData : ItemAbilityEvent
{
    public int Id;
    public float Duration;
    public ProjectileMoveType MoveType;
    public float Speed;
    public float Width;
    public float Height;
    public int CollisionCount;
    public float Damage;
    public ImageType ImageType;
    public Sprite Sprite;
    public RuntimeAnimatorController Animator;
    public AudioKey.Item.Effect AudioKey;

    public ItemProjectileData(int id, float duration, ProjectileMoveType moveType, float speed, float width, float height, int collisionCount, float damage, ImageType imageType, int imageId, AudioKey.Item.Effect audioKey)
    {
        Id = id;
        Duration = duration;
        MoveType = moveType;
        Speed = speed;
        Width = width;
        Height = height;
        CollisionCount = collisionCount;
        Damage = damage;
        ImageType = imageType;
        Animator = null;
        Sprite = null;
        AudioKey = audioKey;

#if UNITY_EDITOR
        switch (imageType)
        {
            case ImageType.Animation:
                Animator = Extension.LoadWithAddresssableByGroup<RuntimeAnimatorController>($"{imageId}", "Projectile");
                if (Animator == null)
                    Debug.LogError($"Animator with ID {imageId} not found in Addressables group 'Projectile'");
                break;
            case ImageType.Sprite:
                Sprite = Extension.LoadWithAddresssableByGroup<Sprite>($"{imageId}", "Projectile");
                if (Sprite == null)
                    Debug.LogError($"Sprite with ID {imageId} not found in Addressables group 'Projectile'");
                break;
        }
#endif
    }
    public void OnEvent(PlayerCharacter player)
    {
        var projectile = Manager.Pool.Pop<ProjectileController>();
        projectile.Init(this, player);
    }
}
