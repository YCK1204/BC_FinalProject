using Game.Monster;
using Game.Player;
using System.Collections;
using UnityEngine;

public class DoubleStrike : Skill
{
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _hitDelay = 0.2f;
    [SerializeField] private float _hitDelay2 = 0.4f;
    [SerializeField] private float _hitMultiplier = 1.2f;
    [SerializeField] private float _hitMultiplier2 = 1.4f;

    [SerializeField] private GameObject _SlashPrefab_1;
    [SerializeField] private GameObject _SlashPrefab_2;

    public override void Execute()
    {
        owner.StartCoroutine(DoubleStrikeCoroutine());

        Debug.Log("발동!");
        // owner.Animator.Play("DoubleStrikeAnimation");
    }

    private IEnumerator DoubleStrikeCoroutine()
    {
        float attackSpeed = owner.Data.CombatData.AttackSpeed;
        PlayerManager.Instance.Player.Animator.Play("Attack_1", 0, 0f);

        yield return new WaitForSeconds(_hitDelay / attackSpeed);
        DealAreaDamage(_hitMultiplier);

        PlayerManager.Instance.Player.Animator.Play("Attack_1", 0, 0f);
        yield return new WaitForSeconds(_hitDelay2 / attackSpeed);
        DealAreaDamage(_hitMultiplier2);
    }

    private void DealAreaDamage(float damageMultiplier)
    {
        Vector2 attackCenter = (Vector2)owner.transform.position + new Vector2(owner.StateMachine.FacingSign * _attackRange * 0.5f, 0f);
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackCenter, _attackRange);

        bool hitted = false;
        foreach (var targetCol in hitTargets)
        {
            if (targetCol.transform.IsChildOf(owner.transform)) continue;

            IDamageable target = targetCol.GetComponentInParent<IDamageable>();
            if (target != null)
            {
                hitted = true;
                int damage = Mathf.RoundToInt(owner.Data.CombatData.AttackPower * damageMultiplier);
                target.TakeDamage(damage);
            }
        }

        if (hitted)
        {
            owner.AttackHit();
        }
    }
}