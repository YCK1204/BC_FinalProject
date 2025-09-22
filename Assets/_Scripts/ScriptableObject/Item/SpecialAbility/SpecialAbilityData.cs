using Game.Player;
using UnityEngine;

public abstract class SpecialAbilityData : ScriptableObject
{
    [SerializeField]
    int abilityID;
    public int AbilityID { get { return abilityID; } }
    public abstract void Activate(PlayerCharacter player);
    public abstract void Inactivate(PlayerCharacter player);
}
