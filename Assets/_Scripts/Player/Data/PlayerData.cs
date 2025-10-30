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

        public GameObject JumpEffectPrefab1;
        public GameObject JumpEffectPrefab2;
    }

    [System.Serializable]
    public class PlayerDashData
    {
        public float Duration = 0.25f;
        public float SpeedMultiplier = 7f;
        public float Cooldown = 3f;
        public bool InvincibleDuringDash = true;
        public LayerMask PassThroughLayers;

        public GameObject DashEffectPrefab;
    }

    [System.Serializable]
    public class PlayerCombatData
    {
        public float AttackPower = 10f;
        public float AttackRange = 1.2f;

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

        public GameObject HitEffectPrefab;
        public GameObject HitCriEffectPrefab;
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
        public float KnockbackPower = 1f;

        public string AnimName;
    }

    [Serializable]
    public class PlayerSkillData
    {
        public float QSkillCastingDuration = 0.5f;
        public float QSkillCooldown = 1.0f;
        public float WSkillCastingDuration = 0.0f;
        public float WSkillCooldown = 1.0f;

        public float GetQSkillCooldown(float skillHaste)
        {
            float hasteMultiplier = skillHaste / 100f;
            if (hasteMultiplier <= 0) return QSkillCooldown;
            return QSkillCooldown / hasteMultiplier;
        }

        public float GetWSkillCooldown(float skillHaste)
        {
            float hasteMultiplier = skillHaste / 100f;
            if (hasteMultiplier <= 0) return WSkillCooldown;
            return WSkillCooldown / hasteMultiplier;
        }
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
        public float MaxAwakeningGauge = 100f;
        public float AwakeningOnHit = 10f;

        public float Duration = 10f;
        public float AwakeningAnimDuration = 1.5f;
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
        public PlayerSkillData SkillData = new PlayerSkillData();

        public PlayerData Clone()
        {
            return new PlayerData
            {
                GroundData = new PlayerGroundData
                {
                    BaseSpeed = GroundData.BaseSpeed,
                    WalkSpeedModifier = GroundData.WalkSpeedModifier
                },
                AirData = new PlayerAirData
                {
                    JumpForce = AirData.JumpForce,
                    DoubleJumpForce = AirData.DoubleJumpForce,

                    JumpEffectPrefab1 = AirData.JumpEffectPrefab1,
                    JumpEffectPrefab2 = AirData.JumpEffectPrefab2
                },
                DashData = new PlayerDashData
                {
                    Duration = DashData.Duration,
                    SpeedMultiplier = DashData.SpeedMultiplier,
                    Cooldown = DashData.Cooldown,
                    InvincibleDuringDash = DashData.InvincibleDuringDash,
                    PassThroughLayers = DashData.PassThroughLayers,
                    DashEffectPrefab = DashData.DashEffectPrefab
                },
                CombatData = new PlayerCombatData
                {
                    AttackPower = CombatData.AttackPower,
                    AttackRange = CombatData.AttackRange,
                    AttackSpeed = CombatData.AttackSpeed,
                    ExtraDamage = CombatData.ExtraDamage,
                    CriticalChance = CombatData.CriticalChance,
                    CriticalDamage = CombatData.CriticalDamage,
                    SkillAttck = CombatData.SkillAttck,
                    SkillHaste = CombatData.SkillHaste,
                    CorruptionDuration = CombatData.CorruptionDuration,
                    AttackPowerPercent = CombatData.AttackPowerPercent,
                    SkillAttckPercent = CombatData.SkillAttckPercent
                },
                HurtData = new PlayerHurtData
                {
                    Duration = HurtData.Duration,
                    KnockbackX = HurtData.KnockbackX,
                    KnockbackY = HurtData.KnockbackY,
                    InvincibleDuringHurt = HurtData.InvincibleDuringHurt
                },
                Stats = new PlayerStatsData
                {
                    MaxHP = Stats.MaxHP
                },
                ComboAttackData = new PlayerComboAttackData
                {
                    AttackInfos = new List<AttackInfoData>(ComboAttackData.AttackInfos)
                },
                awakening = new PlayerAwakeningData
                {
                    MaxAwakeningGauge = awakening.MaxAwakeningGauge,
                    AwakeningOnHit = awakening.AwakeningOnHit,
                    Duration = awakening.Duration,
                    AwakeningAnimDuration = awakening.AwakeningAnimDuration
                },
                SkillData = new PlayerSkillData
                {
                    QSkillCastingDuration = SkillData.QSkillCastingDuration,
                    QSkillCooldown = SkillData.QSkillCooldown,
                    WSkillCastingDuration = SkillData.WSkillCastingDuration,
                    WSkillCooldown = SkillData.WSkillCooldown
                }

            };
        }
    }

}