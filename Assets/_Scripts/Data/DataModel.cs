using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public Dictionary<int, ItemData> Base = new Dictionary<int, ItemData>();
    public Dictionary<int, ItemBuffData> Buff = new Dictionary<int, ItemBuffData>();
    public Dictionary<int, ItemProjectileData> Projectile = new Dictionary<int, ItemProjectileData>();
    public Dictionary<int, ItemAreaData> Area = new Dictionary<int, ItemAreaData>();
    public Dictionary<int, ItemSynergyData> Synergy = new Dictionary<int, ItemSynergyData>();
    public Dictionary<int, ItemDescriptionData> Description = new Dictionary<int, ItemDescriptionData>();
    public Dictionary<int, ItemEffectData> Effect = new Dictionary<int, ItemEffectData>();
}
