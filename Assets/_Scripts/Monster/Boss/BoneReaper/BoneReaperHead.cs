using Game.Monster;
using Game.Player;
using System;
using System.Collections;
using UnityEngine;

public class BoneReaperHead : MonoBehaviour, IDamageable
{
    private BoneReaper _owner;
    private Animator _anim;
    private BoxCollider2D _col;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    private Coroutine _hitEffect;
    private Material _originMat;

    public Action OnDie;

    private LayerMask _mask;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _col = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        _mask = LayerMask.GetMask(Layers.Player);
        _originMat = _sr.material;
    }

    public void Init(BoneReaper boneReaper)
    {
        _owner = boneReaper;
    }

    public void TriggerAnimation(string animationName)
    {
        _anim.SetTrigger(animationName);
    }

    public void SetBoolAnimation(string animationName, bool isTrue)
    {
        _anim.SetBool(animationName, isTrue);
    }

    // 행동1: 브레스 뿜기 공격 판정
    #region Breath
    public void Breath()
    {
        _anim.SetTrigger(BoneReaperAnimatorParams.Breath);
    }

    public void BreathAttack(float size)
    {
        //Manager.Audio.Play(AudioKey.Monster.Attack.M_ATK_BOSS_BREATH, transform);

        Collider2D target;

        float t = Mathf.InverseLerp(2f, 5f, size);
        float ySize = (Mathf.Lerp(1.5f, 0.75f, t) / 2) * _owner.BossScale;


        // 지상 0, 0.375 에서 좌우 size 상하 0.375 박스
        Vector3 AttackPos = new Vector3(0, ySize, 0);
        float lr = size * _owner.BossScale;
        float ud = ySize;
        target = Physics2D.OverlapBox(transform.position + AttackPos, new Vector2(lr*2, ud*2), 0, _mask);
#if UNITY_EDITOR
        Debug.DrawLine(transform.position + AttackPos - Vector3.right * lr + Vector3.up * ud, transform.position + AttackPos + Vector3.right * lr + Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + AttackPos - Vector3.right * lr - Vector3.up * ud, transform.position + AttackPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + AttackPos - Vector3.right * lr + Vector3.up * ud, transform.position + AttackPos - Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + AttackPos + Vector3.right * lr + Vector3.up * ud, transform.position + AttackPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
#endif

        if (target != null)
        {
            // 무적 체크
            PlayerCharacter pc = target.GetComponent<PlayerCharacter>();
            if (pc != null && pc.Invincible)
                return;

            float knockBackDirX = target.transform.position.x < AttackPos.x ? -1 : 1;
            Vector2 knockBackDir = new Vector2(knockBackDirX, 0.5f);
            knockBackDir.Normalize();

            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.linearVelocity = Vector2.zero;
                targetRb.AddForce(knockBackDir * 400);
            }

            target.GetComponent<IDamageable>()?.TakeDamage(_owner.MonsterData.AttackPower, _owner.gameObject);
            Debug.Log((_owner.MonsterData.AttackPower));
        }
    }

    public void EndBreath()
    {
        _owner.IsAttacking = false;
    }

    #endregion
    // 행동2: 오브 생성하기
    public void SummonOrb()
    {
        _anim.SetTrigger(BoneReaperAnimatorParams.SummonHex);
        StartCoroutine(SummonOrbsSequence());
    }

    private IEnumerator SummonOrbsSequence()
    {
        string vertical = "Vertical";
        string spike = "Spike";

        WaitForSeconds orbCreateCoolTime = new WaitForSeconds(0.5f);
        // 7 5 3
        // Todo: 오브젝트 풀로 변경 필요할 듯?
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * 7 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * -7 * _owner.BossScale, vertical);
        //Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * 7 * _owner.BossScale, Quaternion.identity).Init(_owner);
        //Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * -7 * _owner.BossScale, Quaternion.identity).Init(_owner);
        yield return orbCreateCoolTime;

        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * 5 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * -5 * _owner.BossScale, vertical);
        //Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * 5 * _owner.BossScale, Quaternion.identity).Init(_owner);
        //Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * -5 * _owner.BossScale, Quaternion.identity).Init(_owner);
        yield return orbCreateCoolTime;

        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * 3 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * -3 * _owner.BossScale, vertical);
        //Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * 3 * _owner.BossScale, Quaternion.identity).Init(_owner);
        //Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * -3 * _owner.BossScale, Quaternion.identity).Init(_owner);
        yield return orbCreateCoolTime;

        _owner.OrbPool[spike].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.up * -2.5f * _owner.BossScale, spike);
        //Instantiate(_owner.SpikeOrb, transform.position + Vector3.up * -2.5f * _owner.BossScale, Quaternion.identity).Init(_owner);

        yield return null;
    }

    public void ChangePhase()
    {
        _anim.SetInteger("CurPhase", 2);

        _owner.IsDirecting = false;
        _owner.IsInvincible = false;
    }

    public void SummonOrbPhase2()
    {
        _anim.SetTrigger(BoneReaperAnimatorParams.SummonHex);
        StartCoroutine(SummonOrbsSequencePhase2());
    }

    private IEnumerator SummonOrbsSequencePhase2()
    {
        string vertical = "Vertical";

        WaitForSeconds orbCreateCoolTime = new WaitForSeconds(0.5f);
        // 7 5 3
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * 8 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * -8 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * 6 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * -6 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * 4 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * -4 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * 2 * _owner.BossScale, vertical);
        _owner.OrbPool[vertical].Pop().GetComponent<Orb>().Init(_owner, transform.position + Vector3.right * -2 * _owner.BossScale, vertical);

        yield return orbCreateCoolTime;
        _owner.IsAttacking = false;
    }

    // 기타 메서드
    public void TakeDamage(float damage, bool hitSfxMute = false)
    {
        if (_owner.MonsterData.CurHp <= 0 || _owner.IsInvincible)
            return;

        _owner.IsInvincible = true;
        _owner.HitFlash(_sr, _hitEffect, _originMat);
        _owner.TakeDamage(damage, hitSfxMute);
        DamageIndicator.Instance.GetDamage(transform.position + Vector3.up * 4f + Vector3.left * 0.2f, damage);
    }

    public void Die()
    {
        _anim.SetTrigger(BoneReaperAnimatorParams.Die);
        StopAllCoroutines();
    }

    public void DeathEvent()
    {
        OnDie();
    }

    public void PlayLoar()
    {
        Manager.Audio.Play(AudioKey.Direction.DIR_BOSS_START, transform);
    }

    public void PlayLaser()
    {
        Manager.Audio.Play(AudioKey.Monster.Attack.M_ATK_BOSS_LASER, transform);
    }

    public void EndLaser()
    {
        Manager.Audio.StopLoop(transform);
    }

    public void PlayBossSfx(AudioKey.Monster.Attack sfx)
    {
        Manager.Audio.Play(sfx, transform);
    }

    public IEnumerator DelayAnimation(float delay)
    {
        _anim.speed = 0;
        yield return new WaitForSeconds(delay);
        _anim.speed = 1;
    }
}
