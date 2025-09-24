using UnityEngine;

namespace Game.Player
{
    public static class ItemSetterUtil
    {
        public static void SetStat(PlayerCharacter player, ItemStat stat)
        {
            if (stat == null || stat.ItemStatType == ItemStatType.none || stat.Value == 0)
                return;
            switch (stat.ItemStatType)
            {
                case ItemStatType.attack:
                    break;
                case ItemStatType.attackSpeed:
                    break;
                case ItemStatType.HP:
                    break;
                case ItemStatType.criticalChance:
                    break;
                case ItemStatType.criticalDamage:
                    break;
                case ItemStatType.skillAttack:
                    break;
                case ItemStatType.skillHaste:
                    break;
            }
        }
    }
}
