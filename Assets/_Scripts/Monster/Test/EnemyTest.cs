using Game.Monster;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public StateMachineMonster monster;
    public BoneReaperHand hand;
    public BoneReaperHead head;

    [SerializeField] int Damage = 10;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            (monster as IDamageable)?.TakeDamage(Damage);
            (hand as IDamageable)?.TakeDamage(Damage);
            (head as IDamageable)?.TakeDamage(Damage);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            monster.OnDied += () => { Debug.Log(monster.MonsterCount); };
        }
    }

}
