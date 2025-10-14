using Game.Player;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    protected PlayerCharacter owner;
    public virtual void Initialize(PlayerCharacter p)
    {
        this.owner = p;
    }
    public abstract void Execute();
}