using UnityEngine;

public class SplitOnDeath : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    public void Init(BaseMonster monster)
    {
        monster.OnDeath += () =>
        {
            // 하위 몬스터 2마리 생성
            StatModifier stat = new StatModifier(0.5f, 0.5f, 0.5f, 0.5f);
            Instantiate(monster, monster.transform.position + Vector3.right * 0.5f + Vector3.up * 0.5f, Quaternion.identity).MonsterData.SetStatModifier(stat);
            Instantiate(monster, monster.transform.position + Vector3.right * -0.5f + Vector3.up * 0.5f, Quaternion.identity).MonsterData.SetStatModifier(stat);
        };
    }
}
