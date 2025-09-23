using System;
using UnityEngine;

namespace Game.Player
{
    public static class ItemSetterUtil
    {
        public static void ApplyStat(PlayerCharacter player, ItemStatType itemStatType, float value)
        {
            switch (itemStatType)
            {
                case ItemStatType.Attack:
                    player.Data.CombatData.AttackPowerPercent += (value / 100f);
                    break;
                case ItemStatType.SkillAttack:
                    player.Data.CombatData.SkillAttckPercent += (value / 100f);
                    break;
                case ItemStatType.AttackSpeed:
                    player.Data.CombatData.AttackSpeed += (value / 100f);
                    break;
                case ItemStatType.SkillHaste:
                    player.Data.CombatData.SkillHaste += (value / 100f);
                    break;
                case ItemStatType.Hp:
                    player.Data.Stats.MaxHP += value;
                    player.CurrentHP += value;
                    break;
                case ItemStatType.CorruptionDuration:
                    player.Data.CombatData.CorruptionDuration += value;
                    break;
                case ItemStatType.CriticalDamage:
                    player.Data.CombatData.CriticalDamage += value;
                    break;
                case ItemStatType.CriticalChance:
                    player.Data.CombatData.CriticalChance += value;
                    break;
            }
        }
        public static void SetMaxHP(PlayerCharacter player, float val)
        {
            var newMaxHp = player.Data.Stats.MaxHP + val;
            var diff = newMaxHp - player.Data.Stats.MaxHP;

            player.Data.Stats.MaxHP += diff;
            player.CurrentHP += diff;
        }
    }
}
