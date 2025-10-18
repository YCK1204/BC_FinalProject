using UnityEngine;

[CreateAssetMenu(fileName = "New Item Box Audio Data", menuName = "ScriptableObject/Audio/Item/Box Audio Data")]
public class ItemBoxAudioData : AudioData
{
    [SerializeField]
    AudioKey.Item.Box audioKey;
    public AudioKey.Item.Box AudioKey => audioKey;
}
