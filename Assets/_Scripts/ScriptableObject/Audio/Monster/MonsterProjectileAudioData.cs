using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Projectile Audio Data", menuName = "ScriptableObject/Audio/Monster/Projectile Audio Data")]
public class MonsterProjectileAudioData : AudioData
{
    [SerializeField]
    AudioKey.Monster.Projectile audioKey;
    public AudioKey.Monster.Projectile AudioKey => audioKey;
}
