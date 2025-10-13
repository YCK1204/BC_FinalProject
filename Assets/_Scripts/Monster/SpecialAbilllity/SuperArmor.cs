using UnityEngine;

public class SuperArmor : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    public void Init(NormalMonster monster)
    {
        monster.IsSuperArmor = true;
    }
}
