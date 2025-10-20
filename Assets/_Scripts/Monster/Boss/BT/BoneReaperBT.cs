using UnityEngine;
using Game.Monster;
using System.Collections.Generic;

public class BoneReaperBT : BossBT
{
    public override void Init(BossMonster bossMonster)
    {
        _owner = bossMonster;
        SetNodes();
    }

    private SelectorNode _attackSelector;

    /*
    BT 구성
    셀렉터
    ㄴ 타겟 없으면 - 플레이어 탐색
    ㄴ 타겟 있으면 - 공격 패턴 서브 트리

    공격 패턴 서브 트리
    셀렉터 노드
    //ㄴ 쿨타임 중이라면? - 성공 반환
    ㄴ 내려찍기 2스택이라면? - 레이저 공격
    ㄴ 브레스 2스택이라면? - 오브 공격
    ㄴ 랜덤 셀렉터
        ㄴ 내려찍기 공격
        ㄴ 브레스 공격

    랜덤 셀렉터
    ㄴ손
        ㄴ2스택 이하면 내려찍ㄱ
        ㄴ 레이저
    ㄴ머리
        ㄴ2스택 브레스
        ㄴ 오브 공격

    셀렉터
    ㄴ손
        ㄴ2스택 이하면 내려찍ㄱ
        ㄴ 레이저
    ㄴ머리
        ㄴ2스택 브레스
        ㄴ 오브 공격

    ##바꿀 상태##

    셀렉터
    ㄴ 보스 페이즈 변경 트리
    ㄴ 보스 타겟 탐색 트리
    ㄴ 보스 공격 서브 트리

    보스 상태 점검트리
    시퀀스 노드
    ㄴ 체력이 50% 이하이고 1페이즈 인가?
    ㄴ 공격 서브 트리를 2페이즈 트리로 변경
    ㄴ 무적
    ㄴ 포효 패턴 수행

    플레이어 탐색 트리
    셀렉터 노드
    ㄴ 타겟이 존재 하는가?
    ㄴ 타겟 탐색하기

    보스 공격 서브 트리
    셀렉터 노드
    ㄴ 시퀀스 노드
        ㄴ 휘두르기 2스택 이상인가?
        ㄴ 레이저 공격 실행
    ㄴ 시퀀스 노드
        ㄴ 브레스 2스택 이상인가?
        ㄴ 오브 공격 실행
    ㄴ 랜덤 셀렉터 노드
        ㄴ 휘두르기 공격
        ㄴ 브레스 공격

     */

    private void SetNodes1()
    {
        BoneReaper boneReaper = _owner as BoneReaper;
        _root = new SelectorNode("RootNode");

        if (boneReaper == null)
            return;

        // 타겟 없으면?
        ConditionNode checkTarget = new ConditionNode(() => { return _owner.Target == null; }, "TargetIsNull");
        ActionNode findPlayer = new ActionNode(boneReaper.FindTarget, "FindPlayer");
        InvertNode invertFindTarget = new InvertNode(findPlayer, "InvertFindTarget");

        SequenceNode findTargetSequence = new SequenceNode("FindPlayerSequence");
        findTargetSequence.AddChild(checkTarget);
        findTargetSequence.AddChild(invertFindTarget);

        // 타겟 있으면?
        SelectorNode attackSlector = new SelectorNode("AttackSelector");

        // 공격 중인가?
        ConditionNode isAttacking = new ConditionNode(() => { return boneReaper.IsAttacking; }, "IsAttacking");

        // 패턴 쿨타임 중인가?
        ConditionNode isCoolTime = new ConditionNode(() => { return boneReaper.PatternCoolTime < boneReaper.PatternMaxCoolTime; }, "IsCoolTime");

        // 레이저 공격 시퀀스
        SequenceNode laserAttackSequence = new SequenceNode("LaserAttackSequence");
        ConditionNode isSlamMoreThan2 = new ConditionNode(() => { return boneReaper.CurSlamCount >= 2; }, "CheckLaser");
        ActionNode laserAttack = new ActionNode(boneReaper.LaserAttack, "LaserAttack");

        // 오브 공격 시퀀스
        SequenceNode summonOrbSequence = new SequenceNode("SummonOrbSequence");
        ConditionNode isBreathMoreThan2 = new ConditionNode(() => { return boneReaper.CurBreathCount >= 2; }, "CheckSummonOrb");
        ActionNode summonOrbAttack = new ActionNode(boneReaper.SummonOrbAttack, "SummonOrbAttack");

        // 일반 공격 랜덤 셀렉터
        RandomSelectorNode normalAttackRandomSelector = new RandomSelectorNode("NormalAttackSelector");

        // 내려치기 공격 노드
        ActionNode slamAttack = new ActionNode(boneReaper.TwoHandSlamAttack, "SlamAttack");

        // 브레스 공격 노드
        ActionNode breathAttack = new ActionNode(boneReaper.BreathAttack, "BreathAttack");


        // 신나는 노드 조립 시간
        normalAttackRandomSelector.AddChild(breathAttack);
        normalAttackRandomSelector.AddChild(slamAttack);

        summonOrbSequence.AddChild(isBreathMoreThan2);
        summonOrbSequence.AddChild(summonOrbAttack);

        laserAttackSequence.AddChild(isSlamMoreThan2);
        laserAttackSequence.AddChild(laserAttack);

        //attackSlector.AddChild(isAttacking);
        //attackSlector.AddChild(isCoolTime);
        attackSlector.AddChild(laserAttackSequence);
        attackSlector.AddChild(summonOrbSequence);
        attackSlector.AddChild(normalAttackRandomSelector);

        _root.AddChild(findTargetSequence);
        _root.AddChild(attackSlector);
    }

    private void SetNodes()
    {
        BoneReaper boneReaper = _owner as BoneReaper;

        if (boneReaper == null)
            return;

        // 페이즈 변경 시 전환을 위해 미리 캐싱함
        _attackSelector = new SelectorNode(new List<INode>()
            {
                new SequenceNode(new List<INode>()
                {
                    new ConditionNode(() => { return boneReaper.CurSlamCount >= 2; }, "IsSlamCountMoreThan2"),
                    new ActionNode(boneReaper.LaserAttack, "LaserAttack")
                }, "LaserAttackSequence"),
                new SequenceNode(new List<INode>()
                {
                    new ConditionNode(() => { return boneReaper.CurBreathCount >= 2; }, "IsBreathCountMoreThan2"),
                    new ActionNode(boneReaper.SummonOrbAttack, "SummonOrbAttack")
                }, "SummonOrbAttackSequence"),
                new RandomSelectorNode(new List<INode>()
                {
                    new ActionNode(boneReaper.TwoHandSlamAttack, "SlamAttack"),
                    new ActionNode(boneReaper.BreathAttack, "BreathAttack")
                }, "RandomNormalAttack")
            }, "AttackSubTree");

        // 새로운 BT 적용
        _root = new SelectorNode(new List<INode>()
        {
            new SequenceNode(new List<INode>()
            {
                new ConditionNode(() => {return (boneReaper.MonsterData.CurHp <= boneReaper.MonsterData.MaxHp * 0.5f) && (boneReaper.CurPhase == 1); }, "IsPhase2Ready"),
                new ActionNode(boneReaper.ChangePhase2, "ChangePhase2")
            }, "BossPhaseCheckTree"),
            new SequenceNode(new List<INode>()
            {
                new ConditionNode(() => { return boneReaper.Target == null; }, "TargetIsNull"),
                new InvertNode(new ActionNode(boneReaper.FindTarget, "FindPlayer"), "FindTargetResultInverter")
            }, "TargetSearchTree"),
            _attackSelector
        }, "RootNode");
    }

    public NodeStatus Evaluate()
    {
        return _root.Evaluate();
    }

    public void ChangeAttackSelector()
    {
        BoneReaper boneReaper = _owner as BoneReaper;

        if (boneReaper == null)
            return;

        _attackSelector = new SelectorNode(new List<INode>()
        {
            new SequenceNode(new List<INode>()
                {
                    new ConditionNode(() => { return boneReaper.CurSlamCount >= 2; }, "IsSlamCountMoreThan2"),
                    new ActionNode(boneReaper.AdvLaserAttack, "AdvancedLaserAttack")
                }, "AdvancedLaserAttackSequence"),
                new SequenceNode(new List<INode>()
                {
                    new ConditionNode(() => { return boneReaper.CurBreathCount >= 2; }, "IsBreathCountMoreThan2"),
                    new ActionNode(boneReaper.AdvSummonOrbAttack, "AdvancedSummonOrbAttack")
                }, "AdvancedSummonOrbAttackSequence"),
                new RandomSelectorNode(new List<INode>()
                {
                    new ActionNode(boneReaper.AdvSlamAttack, "AdvancedSlamAttack"),
                    new ActionNode(boneReaper.AdvBreathAttack, "AdvancedBreathAttack")
                }, "RandomAdvancedNormalAttack")
        }
        , "Phase2AttackSubTree");

        _root = new SelectorNode(new List<INode>()
        {
            new SequenceNode(new List<INode>()
            {
                new ConditionNode(() => {return (boneReaper.MonsterData.CurHp <= boneReaper.MonsterData.MaxHp * 0.5f) && (boneReaper.CurPhase == 1); }, "IsPhase2Ready"),
                new ActionNode(boneReaper.ChangePhase2, "ChangePhase2")
            }, "BossPhaseCheckTree"),
            new SequenceNode(new List<INode>()
            {
                new ConditionNode(() => { return boneReaper.Target == null; }, "TargetIsNull"),
                new InvertNode(new ActionNode(boneReaper.FindTarget, "FindPlayer"), "FindTargetResultInverter")
            }, "TargetSearchTree"),
            _attackSelector
        }, "RootNode");
    }
}
