using System.Collections.Generic;
using UnityEngine;

public class Berserk : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    private bool _isBerserk = false;
    private List<BaseMonster> _affectedMonsterList;

    public void Init(BaseMonster monster)
    {
        _affectedMonsterList = new List<BaseMonster>();

        monster.Ondetect += () =>
        {
            if (!_isBerserk)
            {
                _isBerserk = true;
                Collider2D[] monsters = Physics2D.OverlapCircleAll(monster.transform.position, monster.MonsterData.DetectRange, LayerMask.GetMask(Game.Monster.Layers.Monster));
                Debug.Log($"{monster.MonsterData.DetectRange}/{monsters.Length}");

                foreach (Collider2D collider in monsters)
                {
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
                    _affectedMonsterList.Add(affectedMonster);
                }
            }
        };

        monster.OnDied += () =>
        {
            if (_affectedMonsterList != null)
            {
                foreach (BaseMonster affectedMonster in _affectedMonsterList)
                {
                    affectedMonster.MonsterData.RemoveModifierByCaster(monster);
                }
            }
        };
    }
}
