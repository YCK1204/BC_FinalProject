using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "ItemEffectDataReader", menuName = "ScriptableObject/Item/EffectDataReader")]
class ItemEffectDataReader : DataReaderBase<ItemEffectData>
{
    public override void UpdateStats(List<GSTU_Cell> list)
    {
        int Id = 0;
        ConditionType Condition = ConditionType.Always;
        string Nmae = "";
        ItemActionType ActionType = ItemActionType.Always;
        float Chance = 0;
        int SpecialAbilityId = 0;
        AbilityType AbilityType = AbilityType.Area;
        float Cooldown = 0;
        int DescId = 0;

        foreach (var item in list)
        {
            switch (item.columnId)
            {
                case "ID":
                    Id = int.Parse(item.value);
                    break;
                case "Condition":
                    Condition = (ConditionType)System.Enum.Parse(typeof(ConditionType), item.value);
                    break;
                case "Name":
                    Nmae = item.value;
                    break;
                case "Action":
                    ActionType = (ItemActionType)System.Enum.Parse(typeof(ItemActionType), item.value);
                    break;
                case "Chance":
                    Chance = float.Parse(item.value);
                    break;
                case "SpecialAbilityID":
                    SpecialAbilityId = int.Parse(item.value);
                    break;
                case "AbilityType":
                    AbilityType = (AbilityType)System.Enum.Parse(typeof(AbilityType), item.value);
                    break;
                case "Cooldown":
                    Cooldown = float.Parse(item.value);
                    break;
                case "DescID":
                    DescId = int.Parse(item.value);
                    break;
            }
        }
        DataList.Add(new ItemEffectData(Id, Condition, Nmae, ActionType, Chance, SpecialAbilityId, AbilityType, Cooldown, DescId));
    }
}
