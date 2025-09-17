using UnityEngine;

public class ProjectileDataHandler : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _lifeTime;

    public int Damage { get { return _damage; } }
    public float Speed { get { return _speed; } }
    public float RotateSpeed { get { return _rotateSpeed; } }
    public float LifeTime { get { return _lifeTime; } }
}
