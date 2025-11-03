using UnityEngine;

namespace Game.Player
{
    public class PlayerAwakeningState : PlayerBaseState
    {
        private float _duration;

        public PlayerAwakeningState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _duration = _stateMachine.Player.Data.awakening.AwakeningAnimDuration;
            _stateMachine.MovementSpeedModifier = 0f;
            _stateMachine.IsAttacking = false;
            _stateMachine.IsDashing = false;

            _stateMachine.Player.Rb.linearVelocity = Vector2.zero;


            _stateMachine.Player.Animator.Play("Awaken", 0, 0f);
            _stateMachine.Player.SetInvincible(true);

            DisplayManager.Instance.FadeIn();
            PlayerManager.Instance.Player.camShake.Shake(0.15f, 0.15f, 1.6f);
            PlayerManager.Instance.Player.camShake.ShakeAfterDelay(1.1f,5f, 3f, 0.2f);

            PlayerManager.Instance.Player.StartCoroutine(PlayerManager.Instance.Player.OnAwken(1.15f));
        }

        public override void Update()
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0f)
            {
                _stateMachine.Player.ActivateAwakening();

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

        public override void Exit()
        {
            StopAnimation(_stateMachine.Player.AnimationData.AwakeningParameterHash);
            _stateMachine.MovementSpeedModifier = 1f;

            _stateMachine.Player.SetInvincible(false);
            DisplayManager.Instance.FadeOut();
            Debug.Log("페이드아웃");

            PlayerManager.Instance.Player.camShake.Shake(2f, 2f, 0.2f);
        }

        public override void PhysicsUpdate()
        {
            _stateMachine.Player.Rb.linearVelocity = new Vector2(0, _stateMachine.Player.Rb.linearVelocity.y);
        }
    }
}