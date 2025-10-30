using Game.Player;
using System;
using UnityEngine;
public enum ItemEffectCreatePosType
{
    Player,
    NearestEnemy,
    WithInRangeEnemy,
}

public enum DetailPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

[Serializable]
public class ItemAreaData : ItemAbilityEvent
{
    public int Id;
    public ItemEffectCreatePosType CreatePosType;
    public DetailPosition DetailPosition;
    public float DetectionRange;
    public float Radius;
    public float Damage;
    public RuntimeAnimatorController Animator;
    public AudioKey.Item.Effect AudioKey;

    public ItemAreaData(int id, ItemEffectCreatePosType createPosType, float detectionRange, DetailPosition detailPosition, float radius, float damage, int animId, AudioKey.Item.Effect audioKey)
    {
        Id = id;
        CreatePosType = createPosType;
        DetailPosition = detailPosition;
        DetectionRange = detectionRange;
        Radius = radius;
        Damage = damage;
#if UNITY_EDITOR
        Animator = Extension.LoadWithAddresssableByGroup<RuntimeAnimatorController>($"{animId}", "Area");
#endif
        AudioKey = audioKey;
    }

    public void OnEvent(PlayerCharacter player)
    {
        var area = Manager.Pool.Pop<AreaController>();
        area.Init(this, player);
    }
}
