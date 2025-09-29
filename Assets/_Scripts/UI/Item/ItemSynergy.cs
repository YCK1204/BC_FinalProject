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
        private set
        {
            _count = value;
            if (_count >= Data.Count)
            {
                if (!_isActive)
                {
                    _isActive = true;
                    OnSynergy();
                }
            }
        }
    }
    bool _isActive = false;
    public ItemSynergyData Data { get; private set; }
    public ItemSynergy(ItemSynergyData data)
    {
        Data = data;
        _count = 0;
    }
    void OnSynergy()
    {
        var player = PlayerCharacter.Instance;

        switch (Data.ActionType)
        {
            case ItemActionType.Always:
                break;
            case ItemActionType.Kill:
                break;
            case ItemActionType.UsingSkill:
                break;
            case ItemActionType.UsingAttack:
                break;
            case ItemActionType.AttackHit:
                break;
            case ItemActionType.StartRound:
                break;
            case ItemActionType.DashEnd:
                break;
            case ItemActionType.OnSynergy:
                break;
        }
    }
}