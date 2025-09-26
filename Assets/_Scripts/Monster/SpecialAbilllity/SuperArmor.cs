using UnityEngine;

public class SuperArmor : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    public void Init(NormalMonster monster)
    {
        monster.IsSuperArmor = true;
        // Todo: 특수 효과 추가?

        // 몬스터의 체력이 50% 이하가 되면 슈퍼아머를 제거하고 스턴상태로 전환
        monster.OnHit += () =>
        {
            if (monster.MonsterData.CurHp <= monster.MonsterData.Data.MaxHp * 0.5f && monster.IsSuperArmor)
            {
                monster.IsSuperArmor = false;
                // 상태 전환 - 스턴
            }
        };
    }
}
