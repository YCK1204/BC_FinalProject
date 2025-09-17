using UnityEngine;

public class MonsterDataHandler : MonoBehaviour
{
    [SerializeField] MonsterData _data;
    public MonsterData Data {  get { return _data; } set { _data = value; } }

    [SerializeField] protected int _curHp;
    public int CurHp { get { return _curHp; } }

    public float Speed { get { return Data._speed; } }
    public float AttackPower { get { return Data._attackPower; } }
    public float AttackDelay { get { return Data._attackDelay; } }
    public float AttackRange { get { return Data._attackRange; } }
    public float DetectRange { get { return Data._detectRange; } }
    public bool CanMove { get { return Data._canMove; } }

    private void Awake()
    {
        _curHp = Data._maxHp;
    }

    public void TakeDamage(int damage)
    {
        _curHp -= Mathf.Max(0, damage);
    }

}
