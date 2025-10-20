using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Die Audio Data", menuName = "ScriptableObject/Audio/Monster/Die Audio Data")]
public class MonsterDieAudioData : AudioData
{
    [SerializeField]
    AudioKey.Monster.Die audioKey;
    public AudioKey.Monster.Die AudioKey => audioKey;
}
