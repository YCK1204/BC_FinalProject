using Game.Monster;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public BaseMonster monster;

    [SerializeField] int Damage = 10;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            (monster as IDamageable)?.TakeDamage(Damage);
        }
    }

}
