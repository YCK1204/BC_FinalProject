using System.Collections.Generic;
using UnityEngine;

public class Berserk : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    private bool _isBerserk = false;
    private List<BaseMonster> _affectedMonsterList;

    public void Init(BaseMonster monster)
    {
        _affectedMonsterList = new List<BaseMonster>();

        // 몬스터가 플레이어를 발견하면 주변 몬스터에게 버프 부여
        // 일단 체력 제외 공격력 또는 속도 버프 제공
        monster.Ondetect += () =>
        {
            // 한번만 발동하도록 설정
            if (!_isBerserk)
            {
                _isBerserk = true;
                Collider2D[] monsters = Physics2D.OverlapCircleAll(monster.transform.position, monster.MonsterData.DetectRange, LayerMask.GetMask(Game.Monster.Layers.Monster));

                foreach (Collider2D collider in monsters)
                {
                    // 자기 자신은 제외
                    if (collider == monster.Col)
                        continue;

                    BaseMonster affectedMonster = collider.GetComponent<BaseMonster>();
                    int randomStat = Random.Range(0, 2);
                    switch (randomStat)
                    {
                        case 0:
                            affectedMonster?.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Attack, Game.Monster.ModifierType.Add, 5, monster));
                            break;
                        case 1:
                            affectedMonster?.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Speed, Game.Monster.ModifierType.Add, 5, monster));
                            break;
                    }
                    // 버프를 제공한 몬스터를 리스트에 등록
                    _affectedMonsterList.Add(affectedMonster);
                }
            }
        };

        // 해당 몬스터가 죽으면 리스트를 순회하며 버프 제거
        monster.OnDied += () =>
        {
            if (_affectedMonsterList != null)
            {
                foreach (StateMachineMonster affectedMonster in _affectedMonsterList)
                {
                    affectedMonster.MonsterData.RemoveModifierByCaster(monster);
                }
            }
        };
    }
}
