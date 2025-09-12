using UnityEngine;

namespace GameSystem
{
    public class PlayerStateMachine : StateMachine
    {
        public PlayerCharacter Player { get; }
        public Vector2 MovementInput { get; set; }
        public float MovementSpeed { get; private set; }
        public float MovementSpeedModifier { get; set; } = 1f;
        public float JumpForce { get; set; }

        public bool IsAttacking { get; set; }

        public PlayerIdleState IdleState { get; }
        public PlayerWalkState WalkState { get; }
        public PlayerJumpState JumpState { get; }
        public PlayerAirState AirState { get; }
        public PlayerGroundedState GroundedState { get; }
        public PlayerAttackState AttackState { get; }

        public PlayerStateMachine(PlayerCharacter player)
        {
            Player = player;
            MovementSpeed = player.Data.GroundData.BaseSpeed;
            JumpForce = player.Data.AirData.JumpForce;

            IdleState = new PlayerIdleState(this);
            WalkState = new PlayerWalkState(this);
            JumpState = new PlayerJumpState(this);
            AirState = new PlayerAirState(this);
            GroundedState = new PlayerGroundedState(this);
            AttackState = new PlayerAttackState(this);
        }
    }
}
