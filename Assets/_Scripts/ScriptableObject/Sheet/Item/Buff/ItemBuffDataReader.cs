using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBuffDataReader", menuName = "ScriptableObject/Item/BuffDataReader")]
class ItemBuffDataReader : DataReaderBase<ItemBuffData>
{
    public override void UpdateStats(List<GSTU_Cell> list)
    {
        int id = 0;
        float duration = 0;
        int maxCount = 0;
        ItemExtraStatType ability1 = ItemExtraStatType.None;
        float ability1Value = 0;
        ItemExtraStatType ability2 = ItemExtraStatType.None;
        float ability2Value = 0;
        string iconURL = "";
        string name = "";
        int descriptionId = 0;
        int imageId = 0;

        foreach (var item in list)
        {
            switch (item.columnId)
            {
                case "ID":
                    id = int.Parse(item.value);
                    break;
                case "Duration":
                    duration = float.Parse(item.value);
                    break;
                case "MaxCount":
                    maxCount = int.Parse(item.value);
                    break;
                case "Ability1":
                    ability1 = (ItemExtraStatType)System.Enum.Parse(typeof(ItemExtraStatType), item.value);
                    break;
                case "Value1":
                    ability1Value = float.Parse(item.value);
                    break;
                case "Ability2":
                    ability2 = (ItemExtraStatType)System.Enum.Parse(typeof(ItemExtraStatType), item.value);
                    break;
                case "Value2":
                    ability2Value = float.Parse(item.value);
                    break;
                case "IconURL":
                    iconURL = item.value;
                    break;
                case "Name":
                    name = item.value;
                    break;
                case "DescID":
                    descriptionId = int.Parse(item.value);
                    break;
                case "ImageID":
                    imageId = int.Parse(item.value);
                    break;
            }
        }
        DataList.Add(new ItemBuffData(id, duration, maxCount, ability1, ability1Value, ability2, ability2Value, iconURL, name, descriptionId, imageId));
    }
}
