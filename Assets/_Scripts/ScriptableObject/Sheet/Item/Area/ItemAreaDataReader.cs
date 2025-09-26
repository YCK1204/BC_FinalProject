using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemAreaDataReader", menuName = "ScriptableObject/Item/AreaDataReader")]
public class ItemAreaDataReaer : DataReaderBase<ItemAreaData>
{
    public override void UpdateStats(List<GSTU_Cell> list)
    {
        int Id = 0;
        float AnmationDuration = 0;
        ItemCreateAreaPosType CreateAreaPosType = ItemCreateAreaPosType.Player;
        float Radius = 0;
        float Damage = 0;
        int AttackCount = 0;
        foreach (var item in list)
        {
            switch (item.columnId)
            {
                case "ID":
                    Id = int.Parse(item.value);
                    break;
                case "AnimationDuration":
                    AnmationDuration = float.Parse(item.value);
                    break;
                case "CreatePosition":
                    CreateAreaPosType = (ItemCreateAreaPosType)System.Enum.Parse(typeof(ItemCreateAreaPosType), item.value);
                    break;
                case "Radius":
                    Radius = float.Parse(item.value);
                    break;
                case "Damage":
                    Damage = float.Parse(item.value);
                    break;
                case "AttackCount":
                    AttackCount = int.Parse(item.value);
                    break;
            }
        }
        DataList.Add(new ItemAreaData(Id, AnmationDuration, CreateAreaPosType, Radius, Damage, AttackCount));
    }
}
