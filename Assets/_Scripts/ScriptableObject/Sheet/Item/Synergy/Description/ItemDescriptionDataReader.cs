using Game.Monster;
using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Reader", menuName = "ScriptableObject/Item/DescriptionDataReader")]
public class ItemDescriptionDataReader : DataReaderBase<ItemDescriptionData>
{
    public override void UpdateStats(List<GSTU_Cell> list)
    {
        int Id = 0;
        string korean = "";
        foreach (GSTU_Cell cell in list)
        {
            switch (cell.columnId)
            {
                case "ID":
                    {
                        Id = int.Parse(cell.value);
                        break;
                    }
                case "Korean":
                    {
                        korean = cell.value;
                        break;
                    }
            }
        }
        DataList.Add(new ItemDescriptionData(Id, korean));
    }
}
