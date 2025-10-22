using Newtonsoft.Json;
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
public class PlayerJsonData
{
    [JsonProperty("hp")]
    public float Hp = 100f;
    [JsonProperty("awaken")]
    public float Awaken = 0f;
    [JsonProperty("cleared_maps")]
    public List<int> ClearedMaps = new List<int>();
    [JsonProperty("cur_map")]
    public int CurMap = 0;
    [JsonProperty("play_time")]
    public float PlayTime = 0f;
}
public class InventoryJsonData
{
    [JsonProperty("items_id")]
    public List<int> ItemsId = new List<int>();
    [JsonProperty("gold")]
    public int Gold = 0;
}