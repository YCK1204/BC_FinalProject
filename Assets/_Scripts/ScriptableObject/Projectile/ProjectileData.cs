using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Data", menuName = "ScriptableObject/Projectile/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string projectileName;

    [Header("전투 관련")]
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float _lifeTime;

    public string Name { get { return projectileName; } }
    public int Damage { get { return damage; } }
    public float Speed { get { return speed; } }
    public float RotateSpeed { get { return rotateSpeed; } }
    public float LifeTime { get { return _lifeTime; } }
}
