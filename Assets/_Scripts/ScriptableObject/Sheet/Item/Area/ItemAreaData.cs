using Game.Player;
using System;
using UnityEditor;
using UnityEditor.AddressableAssets;
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

    public ItemAreaData(int id, ItemEffectCreatePosType createPosType, float detectionRange, DetailPosition detailPosition, float radius, float damage, int animId)
    {
        Id = id;
        CreatePosType = createPosType;
        DetailPosition = detailPosition;
        DetectionRange = detectionRange;
        Radius = radius;
        Damage = damage;
        Animator = Extension.LoadWithAddresssableByGroup<RuntimeAnimatorController>($"{animId}", "Area");
    }

    public void OnEvent(PlayerCharacter player)
    {
        var area = Manager.Pool.Pop<AreaController>();
        area.Init(this, player);
    }
}
