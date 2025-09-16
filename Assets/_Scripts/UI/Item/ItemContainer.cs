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
    TextMeshProUGUI SynergeCount;
    TextMeshProUGUI SynergeName;

    private void Start()
    {
        Init();
        SetUI(ItemData);
    }

    void Init()
    {
        Name = transform.FindChild<TextMeshProUGUI>(false, "Name");
        Description = transform.FindChild<TextMeshProUGUI>(false, "Description");
        Stat1 = transform.FindChild<TextMeshProUGUI>(false, "Stat1");
        Stat2 = transform.FindChild<TextMeshProUGUI>(false, "Stat2");
        SynergeCount = transform.FindChild<TextMeshProUGUI>(false, "SynergeCount");
        SynergeName = transform.FindChild<TextMeshProUGUI>(false, "SynergeName");
    }
    public void SetUI(ItemData itemData)
    {
        ItemData = itemData;

        Name.color = itemData.TierColor;
        Name.text = itemData.ItemName;

        //Description.text = itemData.ItemDescription;
        // 시너지 텍스트 관리
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
