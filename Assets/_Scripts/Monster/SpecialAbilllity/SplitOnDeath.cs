using System;
using UnityEngine;

public class SplitOnDeath : MonoBehaviour, Game.Monster.ISpecialAbillity
{
    private Action _splitHandler;

    public void Init(NormalMonster monster)
    {
        _splitHandler = () =>
        {
            SpawnChild(monster, Vector3.right * 0.5f + Vector3.up * 0.5f);
            SpawnChild(monster, Vector3.right * -0.5f + Vector3.up * 0.5f);
        };

        monster.OnDied += _splitHandler;
    }

    private void SpawnChild(NormalMonster parent, Vector3 offset)
    {
        NormalMonster child = Instantiate(parent, parent.transform.position + offset, Quaternion.identity);

        child.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Attack, Game.Monster.ModifierType.Multiply, 0.5f, null));
        child.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Hp, Game.Monster.ModifierType.Multiply, 0.5f, null));
        child.MonsterData.AddModifier(new StatModifier(Game.Monster.StatType.Scale, Game.Monster.ModifierType.Multiply, 0.5f, null));
        child.MonsterData.Init();

        // 부모의 OnDied 체인 복사
        if (parent.OnDied != null)
        {
            child.OnDied = (Action)Delegate.Combine(parent.OnDied.GetInvocationList());
            child.OnDied -= _splitHandler;
        }
    }
}