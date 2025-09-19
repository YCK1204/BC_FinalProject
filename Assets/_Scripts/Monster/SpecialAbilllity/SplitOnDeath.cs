using UnityEngine;

public class SplitOnDeath : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    public void Init(BaseMonster monster)
    {
        monster.OnDied += () =>
        {
            // 하위 몬스터 2마리 생성
            BaseMonster monster1 = Instantiate(monster, monster.transform.position + Vector3.right * 0.5f + Vector3.up * 0.5f, Quaternion.identity);
            monster1.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Attack, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster1.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Hp, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster1.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Scale, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster1.MonsterData.Init();

            BaseMonster monster2 = Instantiate(monster, monster.transform.position + Vector3.right * -0.5f + Vector3.up * 0.5f, Quaternion.identity);
            monster2.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Attack, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster2.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Hp, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster2.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Scale, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster2.MonsterData.Init();
        };
    }
}
