using Game.Monster;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public StateMachineMonster monster;

    [SerializeField] int Damage = 10;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            (monster as IDamageable)?.TakeDamage(Damage);
        }
    }

}
