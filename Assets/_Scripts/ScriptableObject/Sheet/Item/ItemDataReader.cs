using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reader", menuName = "ScriptableObject/Item/DataReader")]
public class ItemDataReader : DataReaderBase<ItemData>
{
    public override void UpdateStats(List<GSTU_Cell> list)
    {
        int id = 0;
        string name = "";
        ItemExtraStatType ability1Type = ItemExtraStatType.None;
        int ability1Value = 0;
        ItemExtraStatType ability2Type = ItemExtraStatType.None;
        int ability2Value = 0;
        int itemTalentId = 0;
        int synergyId = 0;
        ItemGradeType itemGradeType = ItemGradeType.Common;
        string iconURL = "";
        int descriptionId = 0;

        for (int i = 0; i < list.Count; i++)
        {
            switch (list[i].columnId)
            {
                case "ID":
                    {
                        id = int.Parse(list[i].value);
                        break;
                    }
                case "Grade":
                    {
                        itemGradeType = Enum.Parse<ItemGradeType>(list[i].value);
                        break;
                    }

                case "Name":
                    {
                        name = list[i].value;
                        break;
                    }
                case "Ability1":
                    {
                        ability1Type = Enum.Parse<ItemExtraStatType>(list[i].value);
                        break;
                    }
                case "Value1":
                    {
                        ability1Value = int.Parse(list[i].value);
                        break;
                    }
                case "Ability2":
                    {
                        ability2Type = Enum.Parse<ItemExtraStatType>(list[i].value);
                        break;
                    }
                case "Value2":
                    {
                        ability2Value = int.Parse(list[i].value);
                        break;
                    }
                case "ItemTalentID":
                    {
                        itemTalentId = int.Parse(list[i].value);
                        break;
                    }
                case "SynergyID":
                    {
                        synergyId = int.Parse(list[i].value);
                        break;
                    }
                case "IconURL":
                    {
                        iconURL = list[i].value;
                        break;
                    }
                case "DescID":
                    {
                        descriptionId = int.Parse(list[i].value);
                        break;
                    }
            }
        }

        DataList.Add(new ItemData(id, itemGradeType, name, ability1Type, ability1Value, ability2Type, ability2Value, itemTalentId, synergyId, iconURL, descriptionId));
    }
}
