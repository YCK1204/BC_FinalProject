using UnityEngine;

public class SuperArmor : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    public void Init(BaseMonster monster)
    {
        monster.IsSuperArmor = true;
        // Todo: 특수 효과 추가?

        monster.OnHit += () =>
        {
            if (monster.MonsterData.CurHp <= monster.MonsterData.Data.MaxHp * 0.5f)
            {
                monster.IsSuperArmor = false;
                // 상태 전환 - 스턴
            }
        };
    }
}
