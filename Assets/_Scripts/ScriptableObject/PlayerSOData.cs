using Game.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSOData", menuName = "ScriptableObject/PlayerSOData", order = 1)]
public class PlayerSOData : ScriptableObject
{
    public string PlayerJsonData = "{}";
    public string InventoryJsonData = "{}";
    public bool IsIntroCompleted = false;

    public void ResetData(bool force = false)
    {
        PlayerJsonData = "{}";
        InventoryJsonData = "{}";
        if (force)
            IsIntroCompleted = false;
    }
}
