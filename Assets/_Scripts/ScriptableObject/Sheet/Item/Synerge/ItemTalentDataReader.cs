using Game.Monster;
using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Reader", menuName = "ScriptableObject/Item/TalentDataReader")]
public class ItemTalentDataReader : DataReaderBase<ItemTalentData>
{
    public override void UpdateStats(List<GSTU_Cell> list)
    {
        int id = 0;
        ItemConditionType condition = ItemConditionType.Always;
        float activationChance = 0f;
        float cooldown = 0f;
        int specialAbilityId = 0;
        ItemActionType actionType = ItemActionType.Always;

        foreach (GSTU_Cell cell in list)
        {
            switch (cell.columnId)
            {
                case "ID":
                    {
                        id = int.Parse(cell.value);
                        break;
                    }
                case "Condition":
                    {
                        condition = Enum.Parse<ItemConditionType>(cell.value);
                        break;
                    }
                case "Action":
                    {
                        actionType = Enum.Parse<ItemActionType>(cell.value);
                        break;
                    }
                case "Chance":
                    {
                        activationChance = float.Parse(cell.value);
                        break;
                    }
                case "SpecialAbilityID":
                    {
                        specialAbilityId = int.Parse(cell.value);
                        break;
                    }
                case "Cooldown":
                    {
                        cooldown = float.Parse(cell.value);
                        break;
                    }
            }
        }
        DataList.Add(new ItemTalentData(id, condition, actionType, activationChance, specialAbilityId, cooldown));
    }
}
