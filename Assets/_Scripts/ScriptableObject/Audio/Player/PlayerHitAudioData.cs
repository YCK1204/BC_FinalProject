using UnityEngine;

[CreateAssetMenu(fileName = "New Player Hit Audio Data", menuName = "ScriptableObject/Audio/Player/Hit Audio Data")]
public class PlayerHitAudioData : AudioData
{
    [SerializeField]
    AudioKey.Player.Hit audioKey;
    public AudioKey.Player.Hit AudioKey => audioKey;
}
