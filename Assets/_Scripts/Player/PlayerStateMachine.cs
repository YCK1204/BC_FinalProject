using UnityEngine;

namespace Game.Player
{
    public class PlayerStateMachine
    {
        public PlayerCharacter Player { get; private set; }
        public bool InputActive { get; set; } = true;

        public Vector2 MovementInput { get; set; }
        public float MovementSpeed { get; set; } = 5f;
        public float MovementSpeedModifier { get; set; } = 1f;

        public bool IsAttacking { get; set; }
        public bool IsDashing { get; set; }
        public bool DashPressed { get; set; }
        public int FacingSign { get; set; } = 1;

        public float DashDuration { get; private set; }
        public float DashSpeedMult { get; private set; }
        public float DashCooldown { get; private set; }
        public bool InvincibleDuringDash { get; private set; }

        public int MaxJumps { get; set; } = 2;
        public int JumpsRemaining { get; set; }

        float _lastDashTime = -999f;
        public int ComboIndex { get; set; }
        public bool ContinueCombo { get; set; }
        public float LastAttackEndTime { get; set; } = -999f;
        public bool AttackInputBuffered { get; set; } = false;
        private const float ComboInputBufferTime = 0.5f;

        private float _lastQSkillTime = -999f;
        private float _lastWSkillTime = -999f;

        public IState QSkillState { get; private set; }
        public IState WSkillState { get; private set; }

        public IState ComboAttackState { get; private set; }
        public IState IdleState { get; private set; }
        public IState WalkState { get; private set; }
        public IState AttackState { get; private set; }
        public IState AirState { get; private set; }
        public IState DashState { get; private set; }
        public IState JumpState { get; private set; }
        public IState DoubleJumpState { get; private set; }
        public IState HurtState { get; private set; }
        public IState DieState { get; private set; }
        public IState AwakeningState { get; private set; }
        public IState AirAttackState { get; private set; }

        IState _currentState;

        public PlayerStateMachine(PlayerCharacter player)
        {
            Player = player;

            MovementSpeed = player.Data.GroundData.BaseSpeed;
            DashDuration = player.Data.DashData.Duration;
            DashSpeedMult = player.Data.DashData.SpeedMultiplier;
            DashCooldown = player.Data.DashData.Cooldown;
            InvincibleDuringDash = player.Data.DashData.InvincibleDuringDash;

            IdleState = new PlayerIdleState(this);
            WalkState = new PlayerWalkState(this);
            ComboAttackState = new PlayerComboAttackState(this);
            AirState = new PlayerAirState(this);
            DashState = new PlayerDashState(this);
            JumpState = new PlayerJumpState(this);
            AirAttackState = new PlayerAirAttackState(this);
            HurtState = new PlayerHurtState(this);
            DieState = new PlayerDieState(this);
            AwakeningState = new PlayerAwakeningState(this);

            QSkillState = new PlayerQSkillState(this);
            WSkillState = new PlayerWSkillState(this);

            JumpsRemaining = MaxJumps;
        }

        public void ChangeState(IState next)
        {
            if (_currentState is PlayerComboAttackState || _currentState is PlayerAirAttackState)
            {
                LastAttackEndTime = Time.time;
            }

            _currentState?.Exit();
            _currentState = next;
            _currentState.Enter();
        }

        public void Tick()
        {
            _currentState?.Update();

            if (!IsAttacking && !IsDashing)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                bool attackInput = kb != null && kb.aKey.wasPressedThisFrame;

                if (attackInput && Time.time < LastAttackEndTime + ComboInputBufferTime)
                {
                    AttackInputBuffered = true;
                }
            }
        }

        public void FixedTick() { _currentState?.PhysicsUpdate(); }

        public bool CanDash() => !IsDashing && (Time.time >= _lastDashTime + Player.Data.DashData.Cooldown);
        public void MarkDashedNow() { _lastDashTime = Time.time; }
        public bool CanUseQSkill() => Time.time >= _lastQSkillTime + Player.Data.SkillData.GetQSkillCooldown(Player.Data.CombatData.SkillHaste);
        public void MarkQSkillUsed() { _lastQSkillTime = Time.time; }
        public bool CanUseWSkill() => Time.time >= _lastWSkillTime + Player.Data.SkillData.GetWSkillCooldown(Player.Data.CombatData.SkillHaste);
        public void MarkWSkillUsed() { _lastWSkillTime = Time.time; }
    }
}