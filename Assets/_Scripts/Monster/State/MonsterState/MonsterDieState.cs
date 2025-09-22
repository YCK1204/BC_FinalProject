using UnityEngine;

public class MonsterDieState : MonsterBaseState
{
    private float _curRetrunTime;
    private float _maxReturnTime;

    public MonsterDieState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Game.Monster.StateType.Die;
    }

    public override void Enter()
    {
        base.Enter();

        _curRetrunTime = 0f;
        _maxReturnTime = 2f;

        _stateMachine.Owner.Anim.SetTrigger(Game.Monster.AnimatorParams.Die);
        _stateMachine.Owner.Rb.bodyType = RigidbodyType2D.Kinematic;
        _stateMachine.Owner.Col.isTrigger = true;
        _stateMachine.Owner.Rb.linearVelocityX = 0f;
    }

    public override void Update()
    {
        if(_curRetrunTime >= _maxReturnTime)
        {
            // Todo: 오브젝트 풀로 리턴
            _stateMachine.Owner.Die();
            return;
        }

        _curRetrunTime += Time.deltaTime;
    }
}
