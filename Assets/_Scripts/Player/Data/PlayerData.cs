using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
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
        public LayerMask PassThroughLayers;
    }

    [System.Serializable]
    public class PlayerCombatData
    {
        public float AttackPower = 10f;
        public float AttackRange = 1.5f;

        public float AttackSpeed = 1f;
        public float ExtraDamage = 0f;

        public float CriticalChance = 5f;
        public float CriticalDamage = 200f;
        
        public float SkillAttck = 10f;

        // 기본 스킬가속 = 100% = 1f
        public float SkillHaste = 1f;

        public float CorruptionDuration = 10f;
        public float AttackPowerPercent = 0f;
        public float SkillAttckPercent = 0f;
    }

    [Serializable]
    public class AttackInfoData
    {
        // 콤보어택
        public int ComboStateIndex = -1;
        public float AttackDuration = 0.5f;
        public float HitTiming = 0.2f;
        public float Force = 2f;
        public float ForceTime = 0.1f;
        public float DamageSet = 1.0f;
        public float ComboTime = 0.7f;
    }

    [Serializable]
    public class PlayerComboAttackData
    {
        public List<AttackInfoData> AttackInfos = new List<AttackInfoData>();

        public AttackInfoData GetAttackInfo(int index)
        {
            if (index >= 0 && index < AttackInfos.Count)
            {
                return AttackInfos[index];
            }
            return null;
        }
    }

    [System.Serializable]
    public class PlayerHurtData
    {
        public float Duration = 0.25f;
        public float KnockbackX = 6f;
        public float KnockbackY = 3f;
        public bool InvincibleDuringHurt = true;
    }

    [Serializable]
    public class PlayerAwakeningData
    {
        public float maxAwakeningGauge = 100f;
        public float awakeningOnHit = 10f;

        public float duration = 10f;
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
        public PlayerHurtData HurtData = new PlayerHurtData();
        public PlayerStatsData Stats = new PlayerStatsData();
        public PlayerComboAttackData ComboAttackData = new PlayerComboAttackData();
        public PlayerAwakeningData awakening = new PlayerAwakeningData();
    }
}
