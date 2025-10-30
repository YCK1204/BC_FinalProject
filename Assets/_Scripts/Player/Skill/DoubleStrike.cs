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

    [SerializeField] private GameObject _slashPrefab;
    [SerializeField] private Vector2 _spawnOffset = new Vector2(1.0f, 0.0f);

    public override void Execute()
    {
        owner.StartCoroutine(DoubleStrikeCoroutine());

        Debug.Log("발동!");
        owner.Animator.Play("SkillW");
        Manager.Audio.Play(AudioKey.Player.Skill.P_SKILL_SWORD2, transform);
    }

    private IEnumerator DoubleStrikeCoroutine()
    {
        float facingSign = owner.StateMachine.FacingSign;

        Vector2 offset = new Vector2(_spawnOffset.x * facingSign, _spawnOffset.y);
        Vector2 spawnPosition = (Vector2)owner.transform.position + offset;

        Quaternion rotation = facingSign > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        PlayerPool.Instance.GetFromPool(_slashPrefab, spawnPosition, rotation);

        yield return new WaitForSeconds(_hitDelay);
        PlayerManager.Instance.Player.camShake.Shake(2f, 0.4f, 0.03f);
        DealAreaDamage(_hitMultiplier);

        yield return new WaitForSeconds(_hitDelay2);
        PlayerManager.Instance.Player.camShake.Shake(1f, 0.2f, 0.04f);
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
                owner.StateMachine.Player.GainAwakeningGauge();
                hitted = true;
                int damage = Mathf.RoundToInt(
                    (owner.Data.CombatData.SkillAttck)
                    * (1f + owner.Data.CombatData.SkillAttckPercent)
                    * damageMultiplier
                );
                target.TakeDamage(damage);

                if (target is BaseMonster baseMonsterTarget && baseMonsterTarget.MonsterData.CurHp <= 0)
                {
                    continue;
                }
            }
        }

        if (hitted)
        {
            owner.AttackHit();
        }
    }
}