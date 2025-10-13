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
        int ItemEffectID = 0;

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
                case "ItemEffectID":
                    ItemEffectID = int.Parse(item.value);
                    break;
            }
        }
        DataList.Add(new ItemSynergyData(Id, Count, ItemEffectID));
    }
}
