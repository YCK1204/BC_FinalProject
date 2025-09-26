using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSynergeDataReader", menuName = "ScriptableObject/Item/SynergeDataReader")]
public class ItemSynergyDataReader : DataReaderBase<ItemSynergyData>
{
    public override void UpdateStats(List<GSTU_Cell> list)
    {
        int Id = 0;
        int Count = 0;
        string Name = "";
        ItemActionType ActionType =  ItemActionType.Always;
        float Chance = 0;
        int SpecialAbilityID = 0;
        float CoolDown = 0;
        int DescID = 0;

        foreach (var item in list)
        {
            switch (item.columnId)
            {
                case "ID":
                    Id = int.Parse(item.value);
                    break;
                case "Count":
                    Count = int.Parse(item.value);
                    break;
                case "Name":
                    Name = item.value;
                    break;
                case "Action":
                    ActionType = (ItemActionType)System.Enum.Parse(typeof(ItemActionType), item.value);
                    break;
                case "Chance":
                    Chance = float.Parse(item.value);
                    break;
                case "SpecialAbilityID":
                    SpecialAbilityID = int.Parse(item.value);
                    break;
                case "Cooldown":
                    CoolDown = float.Parse(item.value);
                    break;
                case "DescID":
                    DescID = int.Parse(item.value);
                    break;
            }
        }
        DataList.Add(new ItemSynergyData(Id, Count, Name, ActionType, Chance, SpecialAbilityID, CoolDown, DescID));
    }
}
