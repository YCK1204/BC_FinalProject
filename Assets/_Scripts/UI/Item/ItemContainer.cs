using TMPro;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    TextMeshPro Name;
    TextMeshPro Description;

    bool _isInit = false;

    void Init()
    {
        if (_isInit)
            return;
        _isInit = true;
        Name = transform.FindChild<TextMeshPro>(false, "Name");
        Description = transform.FindChild<TextMeshPro>(false, "Description");
    }
    public void SetUI(ItemData itemData)
    {
        Init();
        Name.color = itemData.TierColor;
        Name.text = itemData.ItemName;

        Manager.Data.ItemsData.Synergy.TryGetValue(itemData.SynergyId, out ItemSynergyData synergyData);
        Manager.Data.ItemsData.Effect.TryGetValue(itemData.ItemEffectId, out ItemEffectData effectData);
        Manager.Item.Synergies.TryGetValue(itemData.SynergyId, out var itemSynergy);
        Description.text = "";
        if (itemData.Stat1.ItemExtraStatType != ItemExtraStatType.None)
        {
            string stat1Ability = ItemData.ItemExtraStatTypes[itemData.Stat1.ItemExtraStatType];
            Description.text += $"{stat1Ability} +{itemData.Stat1.Value}\n";
        }
        if (itemData.Stat2.ItemExtraStatType != ItemExtraStatType.None)
        {
            string stat2Ability = ItemData.ItemExtraStatTypes[itemData.Stat2.ItemExtraStatType];
            Description.text += $"{stat2Ability} +{itemData.Stat2.Value}\n";
        }
        if (itemSynergy != null)
        {
            Description.text += $"시너지 {itemSynergy.Count}/{synergyData.Count}\n";
            Manager.Data.ItemsData.Effect.TryGetValue(synergyData.ItemEffectID, out ItemEffectData synergeEffect);
            Description.text += $"{synergeEffect.Nmae}\n\n";
            var desc = Manager.Data.ItemsData.Description[synergeEffect.DescId].Korean;
            Description.text += $"{desc.Replace(',', '\n')}\n\n";
        }
        if (effectData != null)
        {
            Manager.Data.ItemsData.Description.TryGetValue(effectData.DescId, out var desc);
            Description.text += desc.Korean.Replace(',', '\n');
        }
    }
}
