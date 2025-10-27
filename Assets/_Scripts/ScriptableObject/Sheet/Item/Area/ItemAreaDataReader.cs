using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemAreaDataReader", menuName = "ScriptableObject/Item/AreaDataReader")]
public class ItemAreaDataReader : DataReaderBase<ItemAreaData>
{
    public override void UpdateStats(List<GSTU_Cell> list)
    {
        int Id = 0;
        ItemEffectCreatePosType CreateAreaPosType = ItemEffectCreatePosType.Player;
        DetailPosition detailPosition = DetailPosition.MiddleCenter;
        float detectionRange = 0;
        float Radius = 0;
        float Damage = 0;
        int AnimId = 0;
        AudioKey.Item.Effect audioKey = AudioKey.Item.Effect.ITEM_EFFECT_HOWLING;
        foreach (var item in list)
        {
            switch (item.columnId)
            {
                case "ID":
                    Id = int.Parse(item.value);
                    break;
                case "CreatePosition":
                    CreateAreaPosType = (ItemEffectCreatePosType)System.Enum.Parse(typeof(ItemEffectCreatePosType), item.value);
                    break;
                case "DetailPosition":
                    detailPosition = (DetailPosition)System.Enum.Parse(typeof(DetailPosition), item.value);
                    break;
                case "DetectionRange":
                    detectionRange = float.Parse(item.value);
                    break;
                case "Radius":
                    Radius = float.Parse(item.value);
                    break;
                case "Damage":
                    Damage = float.Parse(item.value);
                    break;
                case "AnimID":
                    AnimId = int.Parse(item.value);
                    break;
                case "AudioKey":
                    audioKey = (AudioKey.Item.Effect)System.Enum.Parse(typeof(AudioKey.Item.Effect), item.value);
                    break;
            }
        }
        DataList.Add(new ItemAreaData(Id, CreateAreaPosType, detectionRange, detailPosition, Radius, Damage, AnimId, audioKey));
    }
}
