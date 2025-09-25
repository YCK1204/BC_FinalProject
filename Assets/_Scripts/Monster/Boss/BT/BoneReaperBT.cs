using UnityEngine;
using Game.Monster;

public class BoneReaperBT : BossBT
{
    public override void Init(BossMonster bossMonster)
    {
        _owner = bossMonster;
    }

    private void SetNodes()
    {
        _root = new SelectorNode();
    }
}
