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
        Description.text = data.Desc;
    }
}
