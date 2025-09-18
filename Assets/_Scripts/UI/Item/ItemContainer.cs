using TMPro;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    ItemData ItemData;

    TextMeshProUGUI Name;
    TextMeshProUGUI Description;
    TextMeshProUGUI Stat1;
    TextMeshProUGUI Stat2;
    TextMeshProUGUI SynergyCount;
    TextMeshProUGUI SynergyName;

    bool _isInit = false;

    void Init()
    {
        if (_isInit)
            return;
        _isInit = true;
        Name = transform.FindChild<TextMeshProUGUI>(false, "Name");
        Description = transform.FindChild<TextMeshProUGUI>(false, "Description");
        Stat1 = transform.FindChild<TextMeshProUGUI>(false, "Stat1");
        Stat2 = transform.FindChild<TextMeshProUGUI>(false, "Stat2");
        SynergyCount = transform.FindChild<TextMeshProUGUI>(false, "SynergyCount");
        SynergyName = transform.FindChild<TextMeshProUGUI>(false, "SynergyName");
    }
    public void SetUI(ItemData itemData)
    {
        Init();
        ItemData = itemData;

        Name.color = itemData.TierColor;
        Name.text = itemData.ItemName;

        Manager.Data.SynergyDict.TryGetValue(itemData.ItemSetSynergyId, out SynergyData synergyData);
        SynergyCount.text = $"시너지 1/{synergyData.RequiredItemCount}";
        SynergyName.text = synergyData.SynergyName;
        Description.text = synergyData.Description;
        Stat1.text = "";
        Stat2.text = "";
        if (itemData.Stat1.ItemStatType != ItemStatType.None)
        {
            string stat1Ability = ItemData.ItemStatTypes[itemData.Stat1.ItemStatType];
            Stat1.text = $"{stat1Ability} +{itemData.Stat1.Value}";
        }
        if (itemData.Stat2.ItemStatType != ItemStatType.None)
        {
            string stat2Ability = ItemData.ItemStatTypes[itemData.Stat2.ItemStatType];
            Stat2.text = $"{stat2Ability} +{itemData.Stat2.Value}";
        }
    }
}
