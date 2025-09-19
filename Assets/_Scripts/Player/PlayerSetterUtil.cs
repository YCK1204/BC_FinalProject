using UnityEngine;

namespace Game.Player
{
    public static class ItemSetterUtil
    {
        public static void SetAttackPower(PlayerCharacter player, float value)
        {
            player.Data.CombatData.AttackPower = Mathf.Max(0f, value);
        }

        public static void AddAttackPowerFlat(PlayerCharacter player, float add)
        {
            player.Data.CombatData.AttackPower = Mathf.Max(0f, player.Data.CombatData.AttackPower + add);
        }

        public static void SetExtraDamage(PlayerCharacter player, float value)
        {
            player.Data.CombatData.ExtraDamage = Mathf.Max(0f, value);
        }

        public static void AddExtraDamageFlat(PlayerCharacter player, float add)
        {
            player.Data.CombatData.ExtraDamage = Mathf.Max(0f, player.Data.CombatData.ExtraDamage + add);
        }

        public static void SetAttackRange(PlayerCharacter player, float value)
        {
            player.Data.CombatData.AttackRange = Mathf.Max(0f, value);
        }

        public static void SetCriticalChancePercent(PlayerCharacter player, float percent)
        {
            player.Data.CombatData.CriticalChance = Mathf.Max(0f, percent);
        }

        public static void AddCriticalChancePercent(PlayerCharacter player, float addPercent)
        {
            player.Data.CombatData.CriticalChance = Mathf.Max(0f, player.Data.CombatData.CriticalChance + addPercent);
        }

        public static void SetCriticalDamagePercent(PlayerCharacter player, float percent)
        {
            player.Data.CombatData.CriticalDamage = Mathf.Max(0f, percent);
        }

        public static void AddCriticalDamagePercent(PlayerCharacter player, float addPercent)
        {
            player.Data.CombatData.CriticalDamage = Mathf.Max(0f, player.Data.CombatData.CriticalDamage + addPercent);
        }


        public static void SetWalkSpeedModifier(PlayerCharacter player, float value)
        {
            player.Data.GroundData.WalkSpeedModifier = Mathf.Max(0f, value);
        }

        public static void MulWalkSpeedModifier(PlayerCharacter player, float factor)
        {
            if (factor <= 0f) factor = 1f;
            player.Data.GroundData.WalkSpeedModifier *= factor;
        }

        public static void SetJumpForce(PlayerCharacter player, float value)
        {
            player.Data.AirData.JumpForce = Mathf.Max(0f, value);
        }

        public static void SetDoubleJumpForce(PlayerCharacter player, float value)
        {
            player.Data.AirData.DoubleJumpForce = Mathf.Max(0f, value);
        }

        public static void SetDashInvincibleFlag(PlayerCharacter player, bool on)
        {
            player.Data.DashData.InvincibleDuringDash = on;
        }

        public static void SetDashDuration(PlayerCharacter player, float seconds)
        {
            player.Data.DashData.Duration = Mathf.Max(0f, seconds);
        }

        public static void SetDashSpeedMultiplier(PlayerCharacter player, float multiplier)
        {
            player.Data.DashData.SpeedMultiplier = Mathf.Max(0f, multiplier);
        }

        public static void SetDashCooldown(PlayerCharacter player, float seconds)
        {
            player.Data.DashData.Cooldown = Mathf.Max(0f, seconds);
        }

        public static void SetMaxHP(PlayerCharacter player, float newMax, bool keepRatio = true)
        {
            newMax = Mathf.Max(1f, newMax);
            float prevMax = player.Data.Stats.MaxHP;
            float cur = player.CurrentHP;
            float targetHp = cur;

            if (keepRatio && prevMax > 0.0001f)
            {
                float ratio = cur / prevMax;
                targetHp = Mathf.Clamp(newMax * ratio, 0f, newMax);
            }
            else
            {
                targetHp = Mathf.Min(cur, newMax);
            }

            player.Data.Stats.MaxHP = newMax;

            float delta = targetHp - cur;
            if (Mathf.Abs(delta) > 0.0001f)
            {
                if (delta > 0f) player.Heal(delta);
                else player.TakeDamage(Mathf.Abs(delta));
            }
        }

        public static void Heal(PlayerCharacter player, float amount)
        {
            if (amount > 0f) player.Heal(amount);
        }

        public static void Damage(PlayerCharacter player, float amount)
        {
            if (amount > 0f) player.TakeDamage(amount);
        }

        public static void SetInvincible(PlayerCharacter player, bool on)
        {
            player.SetInvincible(on);
        }

        public static void SetHurtInvincible(PlayerCharacter player, bool on)
        {
            player.Data.HurtData.InvincibleDuringHurt = on;
        }

        public static void ResetCorruptionGauge(PlayerCharacter player)
        {
            player.ResetCorruptionGauge();
        }

        public static void AddCorruptionOnHit(PlayerCharacter player)
        {
            player.ReportNormalAttackHit();
        }

        public static void SetLayerCollisionIgnore(PlayerCharacter player, LayerMask mask, bool ignore)
        {
            player.SetLayerCollisionIgnore(mask, ignore);
        }
    }
}
