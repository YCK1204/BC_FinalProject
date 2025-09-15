using UnityEngine;

namespace GameSystem
{
    [System.Serializable]
    public class PlayerGroundData
    {
        public float BaseSpeed = 5f;
        public float WalkSpeedModifier = 1f;
    }

    [System.Serializable]
    public class PlayerAirData
    {
        public float JumpForce = 5f;
        public float DoubleJumpForce = 3f;
    }

    [System.Serializable]
    public class PlayerDashData
    {
        public float Duration = 0.25f;
        public float SpeedMultiplier = 7f;
        public float Cooldown = 3f;
        public bool InvincibleDuringDash = true;
    }

    [System.Serializable]
    public class PlayerCombatData
    {
        public float AttackPower = 10f;
        public float AttackRange = 1.5f;
        public float AttackDuration = 0.2f;
    }

    [System.Serializable]
    public class PlayerStatsData
    {
        public float MaxHP = 100f;
    }

    [System.Serializable]
    public class PlayerData
    {
        public PlayerGroundData GroundData = new PlayerGroundData();
        public PlayerAirData AirData = new PlayerAirData();
        public PlayerDashData DashData = new PlayerDashData();
        public PlayerCombatData CombatData = new PlayerCombatData();
        public PlayerStatsData Stats = new PlayerStatsData();
    }
}
