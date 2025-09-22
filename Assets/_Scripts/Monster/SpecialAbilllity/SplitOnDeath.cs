using UnityEngine;

public class SplitOnDeath : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    public void Init(BaseMonster monster)
    {
        monster.OnDied += () =>
        {
            // 하위 몬스터 2마리 생성
            // Todo: Instantiate를 풀로 바꾸기, 만약 바꾸면 MonoBehaviour도 없앨 수 있을 듯
            BaseMonster monster1 = Instantiate(monster, monster.transform.position + Vector3.right * 0.5f + Vector3.up * 0.5f, Quaternion.identity);
            monster1.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Attack, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster1.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Hp, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster1.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Scale, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster1.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Speed, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster1.MonsterData.Init();
            // 혹시 모르니 만약 하위 자식이 분열기능을 가지고 있으면 제거, 추후에 수정할 가능성 높음
            if (Extension.HasComponent<SplitOnDeath>(monster1.gameObject))
                monster1.OnDied = null;

            BaseMonster monster2 = Instantiate(monster, monster.transform.position + Vector3.right * -0.5f + Vector3.up * 0.5f, Quaternion.identity);
            monster2.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Attack, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster2.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Hp, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster2.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Scale, Game.Monster.ModifierType.Multiply, 0.5f, null));
            monster2.MonsterData.Init();

            if (Extension.HasComponent<SplitOnDeath>(monster2.gameObject))
                monster2.OnDied = null;
        };
    }
}
