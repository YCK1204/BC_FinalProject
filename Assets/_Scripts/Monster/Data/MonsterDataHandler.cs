using UnityEngine;

public class MonsterDataHandler : MonoBehaviour
{
    [SerializeField] protected int _curHp = 25;
    public int CurHp { get { return _curHp; } }

    [SerializeField] protected float _speed = 3f;
    public float Speed { get { return _speed; } }

    [SerializeField] protected float _attackPower = 5f;
    public float AttackPower { get { return _attackPower; } }

    [SerializeField] protected float _attackDelay = 1f;
    public float AttackDelay { get { return _attackDelay; } }

    [SerializeField] protected float _attackRange = 3f;
    public float AttackRange { get { return _attackRange; } }

    [SerializeField] protected float _detectRange = 5f;
    public float DetectRange { get { return _detectRange; } }

    [SerializeField] protected bool _canMove = true;
    public bool CanMove { get { return _canMove; } }

    public void TakeDamage(int damage)
    {
        _curHp -= Mathf.Max(0, damage);
    }

    // Todo: 나중에 구현 될 데이터 관련 설정 처리
    public void SetData()
    {
        
    }
}
