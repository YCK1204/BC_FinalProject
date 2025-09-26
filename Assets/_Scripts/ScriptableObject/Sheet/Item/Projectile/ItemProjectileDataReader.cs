using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemProjectileDataReader", menuName = "ScriptableObject/Item/ProjectileDataReader")]
public class ItemProjectileDataReader : DataReaderBase<ItemProjectileData>
{
    public override void UpdateStats(List<GSTU_Cell> list)
    {
        int id = 0;
        float duration = 0;
        float speed = 0;
        float width = 0;
        float height = 0;
        int collisionCount = 0;
        float damage = 0;

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
                case "Speed":
                    speed = float.Parse(item.value);
                    break;
                case "Width":
                    width = float.Parse(item.value);
                    break;
                case "Height":
                    height = float.Parse(item.value);
                    break;
                case "CollisionCount":
                    collisionCount = int.Parse(item.value);
                    break;
                case "Damage":
                    damage = float.Parse(item.value);
                    break;
            }
        }
        DataList.Add(new ItemProjectileData(id, duration, speed, width, height, collisionCount, damage));
    }
}
