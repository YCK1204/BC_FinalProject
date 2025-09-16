using Game.Monster;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public Monster monster;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            (monster as IDamageable)?.TakeDamage(10);
        }
    }

}
