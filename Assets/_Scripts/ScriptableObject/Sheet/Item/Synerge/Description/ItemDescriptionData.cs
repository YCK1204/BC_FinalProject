using System;

[Serializable]
public struct ItemDescriptionData
{
    public int Id;
    public string Korean;
    public ItemDescriptionData(int id, string korean)
    {
        Id = id;
        Korean = korean;
    }
}
