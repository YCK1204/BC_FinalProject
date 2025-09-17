using UnityEngine;

public class MonsterDataHandler : MonoBehaviour
{
    [SerializeField] MonsterData _data;
    public MonsterData Data {  get { return _data; } set { _data = value; } }

    [SerializeField] protected int _curHp;
    public int CurHp { get { return _curHp; } }

    public float Speed { get { return Data.Speed; } }
    public float AttackPower { get { return Data.AttackPower; } }
    public float AttackDelay { get { return Data.AttackDelay; } }
    public float AttackRange { get { return Data.AttackRange; } }
    public float DetectRange { get { return Data.DetectRange; } }
    public bool CanMove { get { return Data.CanMove; } }

    private void Awake()
    {
        _curHp = Data.MaxHp;
    }

    public void TakeDamage(int damage)
    {
        _curHp -= Mathf.Max(0, damage);
    }

}
