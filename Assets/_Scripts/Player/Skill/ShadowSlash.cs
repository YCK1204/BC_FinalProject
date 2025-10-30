using DG.Tweening;
using Game.Monster;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class ShadowSlash : Skill
    {
        [SerializeField] private float _dashDist = 6.0f;
        [SerializeField] private float _dashDur = 0.2f;
        [SerializeField] private int _hitCnt = 3;
        [SerializeField] private float _hitDelay = 0.2f;
        [SerializeField] private float _initial = 0.05f;
        [SerializeField] private float _hitRadius = 0.7f;
        [SerializeField] private LayerMask _hitLayer;
        [SerializeField] private float _dmgMult = 1f;
        [SerializeField] private float _kbPower = 5f;

        private Tween _dashTween;
        private bool _isDashing = false;

        [SerializeField] private GameObject _duskPrefab;
        [SerializeField] private Vector2 _spawnOffset = new Vector2(1.0f, 0.0f);

        [SerializeField] private GameObject _hitPrefab;

        public override void Execute()
        {
            if (_isDashing) return;
            owner.StartCoroutine(DashRoutine());

            PlayerManager.Instance.Player.camShake.Shake(0.5f, 0.2f, 0.04f);
            owner.Animator.Play("Attack_1_skill");
            Manager.Audio.Play(AudioKey.Player.Skill.P_SKILL_SWORD3, transform);

            float facingSign = owner.StateMachine.FacingSign;
            Vector2 offset = new Vector2(_spawnOffset.x * facingSign, _spawnOffset.y);
            Vector2 spawnPosition = (Vector2)owner.transform.position + offset;
            Quaternion rotation = facingSign > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

            PlayerPool.Instance.GetFromPool(_duskPrefab, spawnPosition, rotation);
        }

        private IEnumerator DashRoutine()
        {
            _isDashing = true;
            owner.SetInvincible(true);

            float facingSign = owner.StateMachine.FacingSign;
            Vector2 startPos = owner.transform.position;
            Vector2 targetPos = startPos + new Vector2(_dashDist * facingSign, 0);

            owner.Rb.linearVelocity = Vector2.zero;

            _dashTween?.Kill();
            _dashTween = owner.Rb.DOMoveX(targetPos.x, _dashDur)
                             .SetEase(Ease.OutQuad);

            float timer = 0f;
            HashSet<IDamageable> detectedTargets = new HashSet<IDamageable>();

            while (timer < _dashDur)
            {
                DetectTargets(facingSign, detectedTargets);

                yield return new WaitForSeconds(_initial);
                timer += _initial;
            }

            if (_dashTween != null && _dashTween.active && _dashTween.IsActive() && _dashTween.IsPlaying())
            {
                yield return _dashTween.WaitForCompletion();
            }

            owner.Rb.linearVelocity = Vector2.zero;

            owner.SetInvincible(false);
            _isDashing = false;
        }

        private void DetectTargets(float facingSign, HashSet<IDamageable> detectedTargets)
        {
            Vector2 hitCenter = (Vector2)owner.transform.position + new Vector2(_hitRadius * facingSign, 0);
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(hitCenter, _hitRadius, _hitLayer);

            foreach (Collider2D collider in hitColliders)
            {
                IDamageable target = collider.GetComponentInParent<IDamageable>();
                if (target != null)
                {
                    if (target is BaseMonster baseMonsterTarget && baseMonsterTarget.MonsterData.CurHp <= 0)
                    {
                        continue;
                    }

                    Transform targetTransform = ((MonoBehaviour)target).transform;

                    if (collider.transform.IsChildOf(owner.transform)) continue;

                    if (!detectedTargets.Contains(target))
                    {
                        detectedTargets.Add(target);
                        owner.StartCoroutine(HitTarget(target, facingSign, collider.attachedRigidbody));

                        float randomZ = Random.Range(0f, 360f);
                        Quaternion randomRot = Quaternion.Euler(0f, 0f, randomZ);
                        Vector3 spawnPos = targetTransform.position;

                        PlayerPool.Instance.GetFromPool(_hitPrefab, spawnPos, randomRot);
                    }
                }
            }
        }

        private IEnumerator HitTarget(IDamageable target, float facingSign, Rigidbody2D targetRb)
        {
            for (int i = 0; i < _hitCnt; i++)
            {
                if (target == null || ((MonoBehaviour)target).gameObject == null)
                {
                    yield break;
                }

                int damage = Mathf.RoundToInt(
                    (owner.Data.CombatData.SkillAttck)
                    * (1f + owner.Data.CombatData.SkillAttckPercent)
                );
                float finalDmg = damage * _dmgMult;

                float chance = Mathf.Max(0f, owner.Data.CombatData.CriticalChance) * 0.01f;
                bool isCrit = Random.value < chance;
                float critMult = isCrit ? 1f + Mathf.Max(0f, owner.Data.CombatData.CriticalDamage) * 0.01f : 1f;

                int dmgToApply = Mathf.RoundToInt(finalDmg * critMult);

                target.TakeDamage(dmgToApply);
                owner.MarkLastHitCritical(isCrit);
                owner.GainAwakeningGauge();
                owner.GainAwakeningGauge();

                //if (targetRb != null)
                //{
                //    BaseMonster baseMonsterTarget = target as BaseMonster;
                //    if (baseMonsterTarget == null || baseMonsterTarget.IsSuperArmor)
                //    {
                //        // ss
                //    }
                //    else
                //    {
                //        Vector2 kbDir = new Vector2(facingSign, 0f).normalized;
                //        targetRb.AddForce(kbDir * _kbPower, ForceMode2D.Impulse);
                //    }
                //}

                if (i < _hitCnt - 1)
                {
                    yield return new WaitForSeconds(_hitDelay);
                }
            }
        }
    }
}