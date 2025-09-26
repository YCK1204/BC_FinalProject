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
        public static void ApplyStat(PlayerCharacter player, ItemStat stat)
        {
            if (stat == null || stat.ItemExtraStatType == ItemExtraStatType.None || stat.Value == 0)
                return;
            switch (stat.ItemExtraStatType)
            {
                case ItemExtraStatType.PlusAttack:
                    player.Data.CombatData.AttackPower += stat.Value;
                    break;
                case ItemExtraStatType.Attack:
                    player.Data.CombatData.AttackPowerPercent += stat.Value;
                    break;
                case ItemExtraStatType.PlusSkillAttack:
                    player.Data.CombatData.SkillAttck += stat.Value;
                    break;
                case ItemExtraStatType.SkillAttack:
                    player.Data.CombatData.SkillAttckPercent += stat.Value;
                    break;
                case ItemExtraStatType.AttackSpeed:
                    player.Data.CombatData.AttackSpeed += stat.Value;
                    break;
                case ItemExtraStatType.SkillHaste:
                    player.Data.CombatData.SkillHaste += stat.Value;
                    break;
                case ItemExtraStatType.HP:
                    SetMaxHP(player, stat.Value);
                    break;
                case ItemExtraStatType.CriticalDamage:
                    player.Data.CombatData.CriticalDamage += stat.Value;
                    break;
                case ItemExtraStatType.CriticalChance:
                    player.Data.CombatData.CriticalChance += stat.Value;
                    break;
                case ItemExtraStatType.AwakenDuration:
                    //player.Data.CombatData.AwakenDuration += stat.Value;
                    break;
                case ItemExtraStatType.PlusSpeed:
                    //player.Data.Stats.MoveSpeed += stat.Value;
                    break;
            }
        }
    }
}
