using UnityEngine;

public class MonsterDataHandler : MonoBehaviour
{
    protected int _curHp = 25;
    public int CurHp { get { return _curHp; } }

    protected float _speed = 3f;
    public float Speed { get { return _speed; } }

    protected float _attackPower = 5f;
    public float AttackPower { get { return _attackPower; } }

    protected float _attackDelay = 1f;
    public float AttackDelay { get { return _attackDelay; } }

    protected float _attackRange = 3f;
    public float AttackRange { get { return _attackRange; } }

    protected float _detectRange = 5f;
    public float DetectRange { get { return _detectRange; } }

    protected bool _canMove = true;
    public bool CanMove { get { return _canMove; } }
}
