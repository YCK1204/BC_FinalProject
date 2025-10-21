using TMPro;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    TextMeshPro Name;
    TextMeshPro Description;
    TextMeshPro Stat1;
    TextMeshPro Stat2;
    TextMeshPro SynergyCount;
    TextMeshPro SynergyName;

    bool _isInit = false;

    void Init()
    {
        if (_isInit)
            return;
        _isInit = true;
        Name = transform.FindChild<TextMeshPro>(false, "Name");
        Description = transform.FindChild<TextMeshPro>(false, "Description");
        Stat1 = transform.FindChild<TextMeshPro>(false, "Stat1");
        Stat2 = transform.FindChild<TextMeshPro>(false, "Stat2");
        SynergyCount = transform.FindChild<TextMeshPro>(false, "SynergyCount");
        SynergyName = transform.FindChild<TextMeshPro>(false, "SynergyName");
    }
    public void SetUI(ItemData itemData)
    {
        Init();
        Name.color = itemData.TierColor;
        Name.text = itemData.ItemName;

        Manager.Data.ItemsData.Synergy.TryGetValue(itemData.SynergyId, out ItemSynergyData synergyData);
        //Manager.Data.SynergyDict.TryGetValue(itemData.SynergyId, out SynergyData synergyData);
        //SynergyCount.text = $"시너지 1/{synergyData.RequiredItemCount}";
        //SynergyName.text = synergyData.SynergyName;
        //Description.text = synergyData.Description;
        Stat1.text = "";
        Stat2.text = "";
        if (itemData.Stat1.ItemExtraStatType != ItemExtraStatType.None)
        {
            string stat1Ability = ItemData.ItemExtraStatTypes[itemData.Stat1.ItemExtraStatType];
            Stat1.text = $"{stat1Ability} +{itemData.Stat1.Value}";
        }
        if (itemData.Stat2.ItemExtraStatType != ItemExtraStatType.None)
        {
            string stat2Ability = ItemData.ItemExtraStatTypes[itemData.Stat2.ItemExtraStatType];
            Stat2.text = $"{stat2Ability} +{itemData.Stat2.Value}";
        }
    }
}
