using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Data", menuName = "ScriptableObject/Monster/Monster Data")]
public class MonsterData : ScriptableObject
{
    public int MaxHp = 25;
    public float Speed = 3f;
    public float AttackPower = 5f;
    public float AttackDelay = 1f;
    public float AttackRange = 3f;
    public float DetectRange = 5f;
    public bool CanMove = true;
}
