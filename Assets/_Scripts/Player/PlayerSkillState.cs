using UnityEngine;

namespace Game.Player
{
    public class PlayerSkillState : PlayerBaseState
    {
        protected float _castingTimer;
        protected float _skillDuration;

        public PlayerSkillState(PlayerStateMachine stateMachine, float skillDuration) : base(stateMachine)
        {
            _skillDuration = skillDuration;
        }

        public override void Enter()
        {
            _castingTimer = _skillDuration;
            _stateMachine.MovementSpeedModifier = 0f;
           
            // StartAnimation(_stateMachine.Player.AnimationData.SkillParameterHash); 
        }

        public override void Exit()
        {
            _stateMachine.MovementSpeedModifier = 1f;
            
            // StopAnimation(_stateMachine.Player.AnimationData.SkillParameterHash);
        }

        public override void Update()
        {
            _castingTimer -= Time.deltaTime;

            if (_castingTimer <= 0f)
            {
                if (_stateMachine.Player.IsGrounded())
                {
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                }
                else
                {
                    _stateMachine.ChangeState(_stateMachine.AirState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            if (_stateMachine.Player.Rb.linearVelocity.x != 0f)
            {
                _stateMachine.Player.Rb.linearVelocity = new Vector2(0f, _stateMachine.Player.Rb.linearVelocity.y);
            }
        }
    }
}