using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Data", menuName = "ScriptableObject/Projectile/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string _projectileName;

    [Header("전투 관련")]
    [SerializeField] private float _speed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _lifeTime;

    public string Name { get { return _projectileName; } }
    public float Speed { get { return _speed; } }
    public float RotateSpeed { get { return _rotateSpeed; } }
    public float LifeTime { get { return _lifeTime; } }
}
