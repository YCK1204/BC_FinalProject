using UnityEngine;

namespace Game.Player
{
    public class PlayerBaseState : IState
    {
        protected PlayerStateMachine _stateMachine;

        public PlayerBaseState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
        public virtual void PhysicsUpdate() { HandleMove(); }

        protected void StartAnimation(int hash) { _stateMachine.Player.Animator.SetBool(hash, true); }
        protected void StopAnimation(int hash) { _stateMachine.Player.Animator.SetBool(hash, false); }

        protected void ReadMoveInput()
        {
            if (!_stateMachine.InputActive)
            {
                _stateMachine.MovementInput = Vector2.zero;
                return;
            }

#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            float x = 0f;
            if (kb != null)
            {
                if (kb.leftArrowKey.isPressed) x -= 1f;
                if (kb.rightArrowKey.isPressed) x += 1f;
            }
#else
            float x = 0f;
            if (Input.GetKey(KeyCode.LeftArrow))  x -= 1f;
            if (Input.GetKey(KeyCode.RightArrow)) x += 1f;
#endif
            _stateMachine.MovementInput = new Vector2(Mathf.Clamp(x, -1f, 1f), 0f);
        }

        protected void HandleMove()
        {
            float x = _stateMachine.MovementInput.x;
            float speed = _stateMachine.MovementSpeed * _stateMachine.MovementSpeedModifier;
#if UNITY_2022_3_OR_NEWER
            var v = _stateMachine.Player.Rb.linearVelocity;
            _stateMachine.Player.Rb.linearVelocity = new Vector2(x * speed, v.y);
#else
            var v = _stateMachine.Player.Rb.velocity;
            _stateMachine.Player.Rb.velocity = new Vector2(x * speed, v.y);
#endif
            if (x != 0f)
            {
                var s = _stateMachine.Player.transform.localScale;
                s.x = Mathf.Abs(s.x) * (x > 0f ? 1f : -1f);
                _stateMachine.Player.transform.localScale = s;
                _stateMachine.FacingSign = x > 0f ? 1 : -1;
            }
        }
    }
}
