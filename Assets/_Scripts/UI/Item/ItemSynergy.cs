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
        private set { _count = value; }
    }
}