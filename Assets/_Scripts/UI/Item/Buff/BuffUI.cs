using TMPro;
using UnityEngine;

public class BuffUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Title;
    [SerializeField]
    TextMeshProUGUI Description;

    public void SetUI(ItemBuffData data)
    {
        Title.text = data.Name;

        if (string.IsNullOrEmpty(data.Desc))
            data.Desc = Manager.Data.ItemsData.Description[data.DescriptionId].Korean;
        Description.text = data.Desc;
    }
}
