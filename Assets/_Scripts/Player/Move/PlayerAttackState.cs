using UnityEngine;

namespace GameSystem
{
    public class PlayerAttackState : PlayerGroundedState
    {
        private float _attackDuration = 0.25f;
        private float _timer;

        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _stateMachine.MovementSpeedModifier = 0f;
            _stateMachine.IsAttacking = true;
            _timer = _attackDuration;

            if (_stateMachine.Player.AnimationData != null)
            {
                StartAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            }
        }

        public override void Exit()
        {
            _stateMachine.IsAttacking = false;

            if (_stateMachine.Player.AnimationData != null)
            {
                StopAnimation(_stateMachine.Player.AnimationData.AttackParameterHash);
            }
        }

        public override void Update()
        {
            base.ReadMoveInput();

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                if (_stateMachine.MovementInput == Vector2.zero)
                {
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                }
                else
                {
                    _stateMachine.ChangeState(_stateMachine.WalkState);
                }
                return;
            }
        }

        public override void PhysicsUpdate()
        {
        }
    }
}
