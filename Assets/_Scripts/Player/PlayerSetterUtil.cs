using System;
using UnityEngine;

namespace Game.Player
{
    public static class ItemSetterUtil
    {
        public static void SetMaxHP(PlayerCharacter player, float val)
        {
            var newMaxHp = player.Data.Stats.MaxHP + val;
            var diff = newMaxHp - player.Data.Stats.MaxHP;

            player.Data.Stats.MaxHP += diff;
            player.CurrentHP += diff;
        }
        public static void ApplyStat(PlayerCharacter player, ItemExtraStatType type, float val)
        {
            if (type == ItemExtraStatType.None || val == 0)
                return;
            switch (type)
            {
                case ItemExtraStatType.PlusAttack:
                    player.Data.CombatData.AttackPower += val;
                    break;
                case ItemExtraStatType.Attack:
                    player.Data.CombatData.AttackPowerPercent += val / 100f;
                    break;
                case ItemExtraStatType.PlusSkillAttack:
                    player.Data.CombatData.SkillAttck += val;
                    break;
                case ItemExtraStatType.SkillAttack:
                    player.Data.CombatData.SkillAttckPercent += val / 100f;
                    break;
                case ItemExtraStatType.AttackSpeed:
                    player.Data.CombatData.AttackSpeed += val / 100f;
                    break;
                case ItemExtraStatType.SkillHaste:
                    player.Data.CombatData.SkillHaste += val;
                    break;
                case ItemExtraStatType.HP:
                    SetMaxHP(player, val);
                    break;
                case ItemExtraStatType.CriticalDamage:
                    player.Data.CombatData.CriticalDamage += val;
                    break;
                case ItemExtraStatType.CriticalChance:
                    player.Data.CombatData.CriticalChance += val;
                    break;
                case ItemExtraStatType.AwakenDuration:
                    player.Data.awakening.Duration += val;
                    break;
                case ItemExtraStatType.PlusSpeed:
                    //player.Data.Stats.MoveSpeed += val;
                    break;
            }
        }
        public static void RemoveStat(PlayerCharacter player, ItemExtraStatType type, float val)
        {
            if (type == ItemExtraStatType.None || val == 0)
                return;
            switch (type)
            {
                case ItemExtraStatType.PlusAttack:
                    player.Data.CombatData.AttackPower -= val;
                    break;
                case ItemExtraStatType.Attack:
                    player.Data.CombatData.AttackPowerPercent -= val / 100f;
                    break;
                case ItemExtraStatType.PlusSkillAttack:
                    player.Data.CombatData.SkillAttck -= val;
                    break;
                case ItemExtraStatType.SkillAttack:
                    player.Data.CombatData.SkillAttckPercent -= val / 100f;
                    break;
                case ItemExtraStatType.AttackSpeed:
                    player.Data.CombatData.AttackSpeed -= val;
                    break;
                case ItemExtraStatType.SkillHaste:
                    player.Data.CombatData.SkillHaste -= val;
                    break;
                case ItemExtraStatType.HP:
                    SetMaxHP(player, -val);
                    break;
                case ItemExtraStatType.CriticalDamage:
                    player.Data.CombatData.CriticalDamage -= val;
                    break;
                case ItemExtraStatType.CriticalChance:
                    player.Data.CombatData.CriticalChance -= val;
                    break;
                case ItemExtraStatType.AwakenDuration:
                    player.Data.awakening.Duration -= val;
                    break;
                case ItemExtraStatType.PlusSpeed:
                    //player.Data.Stats.MoveSpeed -= val;
                    break;
            }
        }
    }
}
