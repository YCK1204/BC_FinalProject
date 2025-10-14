using UnityEngine;

public class Shield : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    public void Init(NormalMonster monster)
    {
        monster.Ondetect += () =>
        {
            return;
        };
    }
}
