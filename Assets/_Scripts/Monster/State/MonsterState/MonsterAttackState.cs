using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Attack;
    }

    public override void Enter()
    {
        base.Enter();

        _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, 0);
        _stateMachine.Owner.Attack.OnAttackEnd += AttackEnd;
        Attack();
    }

    public override void Exit()
    {
        base.Exit();

        _stateMachine.Owner.Attack.OnAttackEnd -= AttackEnd;
    }

    private void Attack()
    {
        _stateMachine.Owner.Anim.SetTrigger(Common.AnimatorParams.Attack);
    }

    private void AttackEnd()
    {
        if (CheckDetectRange())
        {
            // 감지가 되고 공격 범위 내라면 공격
            //if (CheckAttackRange())
            if(_stateMachine.Owner.Attack.Attackable.GetCheckAttackable())
            {
                Attack();
            }
            // 감지는 됬는데 공격 사정거리 밖이라면 추격
            else
            {
                _stateMachine.ChangeState(Common.StateType.Chase);
            }
        }
        else
        {
            _stateMachine.ChangeState(Common.StateType.Idle);
        }
    }
}
