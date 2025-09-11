using UnityEngine;

public abstract class PatrolMonster : Monster
{
    private Game.Monster.IMovable curMovement;

    public void SetMovement(Game.Monster.IMovable movable)
    {
        if (curMovement != movable)
            curMovement = movable;
    }

}
