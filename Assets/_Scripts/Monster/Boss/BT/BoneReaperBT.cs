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
    셀렉터
    ㄴ 타겟 없으면 - 플레이어 탐색
    ㄴ 타겟 있으면 - 공격 패턴 서브 트리

    공격 패턴 서브 트리
    셀렉터 노드
    ㄴ 공격중이라면 종료 -> 공격중이면 성공 반환
    ㄴ 패턴 쿨타임 중이라면 종료 -> 패턴 쿨타임이 최대 이하면 성공 반환
    ㄴ 내려찍기 2스택이라면? - 레이저 공격
    ㄴ 브레스 2스택이라면? - 오브 공격
    ㄴ 랜덤 셀렉터
        ㄴ 내려찍기 공격
        ㄴ 브레스 공격
     */

    private void SetNodes()
    {
        BoneReaper boneReaper = _owner as BoneReaper;
        _root = new SelectorNode();

        if (boneReaper == null)
            return;

        // 타겟 없으면?
        ConditionNode checkTarget = new ConditionNode(() => { return _owner.Target != null; });
        ActionNode findPlayer = new ActionNode(boneReaper.FindTarget);

        SequenceNode findTargetSequence = new SequenceNode();
        findTargetSequence.AddChild(checkTarget);
        findTargetSequence.AddChild(findPlayer);

        // 타겟 있으면?
        SelectorNode attackSlector = new SelectorNode();

        // 공격 중인가?
        ConditionNode isAttacking = new ConditionNode(() => { return boneReaper.IsAttacking; });

        // 레이저 공격 시퀀스
        SequenceNode laserAttackSequence = new SequenceNode();
        ConditionNode isSlamMoreThan2 = new ConditionNode(() => { return boneReaper.CurSlamCount >= 2; });
    }

    public NodeStatus Evaluate()
    {
        return _root.Evaluate();
    }
}
