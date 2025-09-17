using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Data", menuName = "Data/Monster Data")]
public class MonsterData : ScriptableObject
{
    public int _maxHp = 25;
    public float _speed = 3f;
    public float _attackPower = 5f;
    public float _attackDelay = 1f;
    public float _attackRange = 3f;
    public float _detectRange = 5f;
    public bool _canMove = true;
}
