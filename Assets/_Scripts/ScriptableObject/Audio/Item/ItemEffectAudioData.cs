using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect Audio Data", menuName = "ScriptableObject/Audio/Item/Effect Audio Data")]
public class ItemEffectAudioData : AudioData
{
    [SerializeField]
    AudioKey.Item.Effect audioKey;
    public AudioKey.Item.Effect AudioKey => audioKey;
}
