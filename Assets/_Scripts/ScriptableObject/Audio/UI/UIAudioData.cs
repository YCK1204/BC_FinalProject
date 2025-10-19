using UnityEngine;

[CreateAssetMenu(fileName = "New UI Audio Data", menuName = "ScriptableObject/Audio/UI/UI Audio Data")]
public class UIAudioData : AudioData
{
    [SerializeField]
    AudioKey.UI audioKey;
    public AudioKey.UI AudioKey => audioKey;
}
