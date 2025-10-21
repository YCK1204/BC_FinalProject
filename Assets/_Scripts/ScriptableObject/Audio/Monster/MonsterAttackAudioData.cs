using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Attack Audio Data", menuName = "ScriptableObject/Audio/Monster/Attack Audio Data")]
public class MonsterAttackAudioData : AudioData
{
    [SerializeField]
    AudioKey.Monster.Attack audioKey;
    public AudioKey.Monster.Attack AudioKey => audioKey;
}
