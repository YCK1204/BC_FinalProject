using System;

public enum SynergyEffectType
{
    Duration,
    Always,
    Creation
}

[Serializable]
public class ItemSynergyData
{
    public int Id;
    public int Count;
    public int ItemEffectID;
    public ItemSynergyData(int id, int count, int itemEffectID)
    {
        Id = id;
        Count = count;
        ItemEffectID = itemEffectID;
    }
}
