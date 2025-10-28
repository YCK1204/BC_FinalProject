using UnityEngine;

public class MonsterHitState : MonsterBaseState
{
    private float _curHitTime;
    private float _maxHitTime;

    public MonsterHitState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Game.Monster.StateType.Hit;
    }

    public override void Enter()
    {
        base.Enter();

        _maxHitTime = 0.5f;
        _curHitTime = 0f;
        //_stateMachine.Owner.Anim.SetFloat(Game.Monster.AnimatorParams.Speed, 0);
        // Hit 트리거 리셋
        _stateMachine.Owner.Anim.ResetTrigger(Game.Monster.AnimatorParams.Hit);
        _stateMachine.Owner.Anim.SetTrigger(Game.Monster.AnimatorParams.Hit);
        //_stateMachine.Owner.Rb.linearVelocityX = 0f;
    }

    // Todo: 피격 시, 몬스터 행동 불가 / 일정 시간 이후 대기 상태로 복귀

    public override void Update()
    {
        base.Update();
        if(_curHitTime >= _maxHitTime)
        {
            if (_stateMachine.Owner.Target == null)
                _stateMachine.Owner.transform.localScale = new Vector3(_stateMachine.Owner.transform.localScale.x * -1, _stateMachine.Owner.transform.localScale.y, _stateMachine.Owner.transform.localScale.z);
            _stateMachine.ChangeState(Game.Monster.StateType.Idle);
            return;
        }
        _curHitTime += Time.deltaTime;
    }
}
