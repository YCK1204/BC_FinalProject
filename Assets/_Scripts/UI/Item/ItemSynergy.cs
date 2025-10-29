using Game.Player;
using UnityEngine;

public enum SynergyType
{
    Buff,
    Projectile,
    Area
}

public class ItemSynergy
{
    int _count = 0;
    public int Count
    {
        get { return _count; }
        set
        {
            _count = value;
            if (_count >= Data.Count)
            {
                if (!Activated)
                {
                    Activated = true;
                    OnSynergy();
                }
            }
        }
    }
    public bool Activated = false;
    public ItemSynergyData Data { get; private set; }
    public ItemSynergy(ItemSynergyData data)
    {
        Data = data;
        _count = 0;
    }
    void OnSynergy()
    {
        var player = PlayerCharacter.Instance;
        Manager.Data.ItemsData.Effect.TryGetValue(Data.ItemEffectID, out var effectData);

        if (effectData == null)
        {
            Debug.LogError($"ItemEffectId {Data.Id} not found");
            return;
        }
        effectData.Set();
    }
}