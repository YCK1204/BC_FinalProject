using UnityEngine;

[CreateAssetMenu(fileName = "New Synergy Data", menuName = "ScriptableObject/Item/Synergy Data")]
public class SynergyData : ScriptableObject
{
    [SerializeField]
    int id;
    public int Id { get { return id; } }
    [SerializeField]
    string synergyName;
    public string SynergyName { get { return synergyName; } }
    [SerializeField, TextArea]
    string description;
    public string Description { get { return description; } }
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get { return icon; } }
    [SerializeField]
    int requiredItemCount;
    public int RequiredItemCount { get { return requiredItemCount; } }
}
