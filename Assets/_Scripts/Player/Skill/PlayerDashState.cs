using UnityEngine;

namespace Game.Player
{
    public class PlayerDashState : PlayerBaseState
    {
        float _timer;
        Vector2 _dashDir;
        float _prevGravity;

        float _spawnTimer;
        float DefaultSpawnInterval = 0.05f;
        GameObject _dashEffectPrefab;

        public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            PlayerManager.Instance?.CooldownS?.StartCooldown(PlayerManager.Instance.Player.Data.DashData.Cooldown);

            _stateMachine.IsDashing = true;
            _timer = _stateMachine.DashDuration;
            _dashDir = new Vector2(_stateMachine.FacingSign, 0f);

            _prevGravity = _stateMachine.Player.Rb.gravityScale;
            _stateMachine.Player.Rb.gravityScale = 0f;

            var v = _stateMachine.Player.Rb.linearVelocity;
            _stateMachine.Player.Rb.linearVelocity = new Vector2(v.x, 0f);


            _stateMachine.MovementSpeedModifier = _stateMachine.DashSpeedMult;
            StartAnimation(_stateMachine.Player.AnimationData.DashParameterHash);
            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);

            if (_stateMachine.InvincibleDuringDash)
                _stateMachine.Player.SetInvincible(true);

            _stateMachine.Player.SetLayerCollisionIgnore(_stateMachine.Player.Data.DashData.PassThroughLayers, true);

            _stateMachine.MarkDashedNow();

            _dashEffectPrefab = _stateMachine.Player.Data.DashData.DashEffectPrefab;
            _spawnTimer = 0f;

            Manager.Audio.Play(AudioKey.Player.Move.P_MOVE_JUMP_START, _stateMachine.Player.transform);
        }

        public override void Exit()
        {
            _stateMachine.Player.DashEnd();

            _stateMachine.IsDashing = false;
            _stateMachine.MovementSpeedModifier = 1f;
            _stateMachine.Player.Rb.gravityScale = _prevGravity;

            StopAnimation(_stateMachine.Player.AnimationData.DashParameterHash);

            if (_stateMachine.InvincibleDuringDash)
                _stateMachine.Player.SetInvincible(false);

            _stateMachine.Player.SetLayerCollisionIgnore(_stateMachine.Player.Data.DashData.PassThroughLayers, false);
        }

        public override void Update()
        {
            _stateMachine.MovementInput = _dashDir;

            if (_dashEffectPrefab != null)
            {
                _spawnTimer -= Time.deltaTime;
                if (_spawnTimer <= 0f)
                {
                    _spawnTimer = DefaultSpawnInterval;
                    SpawnDashEffect();
                }
            }

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                if (_stateMachine.Player.IsGrounded())
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                else
                    _stateMachine.ChangeState(_stateMachine.AirState);
            }
        }

        void SpawnDashEffect()
        {
            Vector3 pos = _stateMachine.Player.transform.position;

            Quaternion rot = Quaternion.identity;
            if (_stateMachine.FacingSign < 0)
            {
                rot = Quaternion.Euler(0f, 180f, 0f);
            }

            PlayerPool.Instance.GetFromPool(_dashEffectPrefab, pos, rot);
        }
    }
}
