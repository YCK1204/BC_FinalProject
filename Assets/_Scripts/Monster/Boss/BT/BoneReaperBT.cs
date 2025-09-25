using UnityEngine;
using Game.Monster;

public class BoneReaperBT : BossBT
{
    public override void Init(BossMonster bossMonster)
    {
        _owner = bossMonster;
        SetNodes();
    }

    /*
    BT 구성

     */

    private void SetNodes()
    {
        _root = new SelectorNode();
        ConditionNode checkTarget = new ConditionNode(() => { return _owner.Target == null; });
        //ActionNode FindPlayer = new ActionNode();
    }

    public NodeStatus Evaluate()
    {
        return _root.Evaluate();
    }
}
