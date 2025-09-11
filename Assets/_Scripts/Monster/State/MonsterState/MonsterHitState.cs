using UnityEngine;

public class MonsterHitState : MonsterBaseState
{
    public MonsterHitState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Hit;
    }

    // Todo: 피격 시, 몬스터 행동 불가 / 일정 시간 이후 이전 상태로 복귀
}
