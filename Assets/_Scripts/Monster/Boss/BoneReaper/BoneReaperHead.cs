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

    public Action OnDie;

    private LayerMask _mask;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _col = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        _mask = LayerMask.GetMask(Layers.Player);
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
        Collider2D target;

        float t = Mathf.InverseLerp(2f, 5f, size);
        float ySize = Mathf.Lerp(2f, 1f, t) / 2;


        // 지상 0, 0.5 에서 좌우 size 상하 0.5 박스
        Vector3 AttackPos = new Vector3(0, ySize, 0);
        float lr = size;
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

            target.GetComponent<IDamageable>()?.TakeDamage((int)(_owner.MonsterData.AttackPower));
            Debug.Log((int)(_owner.MonsterData.AttackPower));
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
        WaitForSeconds orbCreateCoolTime = new WaitForSeconds(0.5f);
        // 7 5 3
        // Todo: 오브젝트 풀로 변경 필요할 듯?
        Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * 7, Quaternion.identity).Init(_owner);
        Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * -7, Quaternion.identity).Init(_owner);
        yield return orbCreateCoolTime;

        Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * 5, Quaternion.identity).Init(_owner);
        Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * -5, Quaternion.identity).Init(_owner);
        yield return orbCreateCoolTime;

        Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * 3, Quaternion.identity).Init(_owner);
        Instantiate(_owner.VerticalOrb, transform.position + Vector3.right * -3, Quaternion.identity).Init(_owner);
        yield return orbCreateCoolTime;

        Instantiate(_owner.SpikeOrb, transform.position + Vector3.up * -2.5f, Quaternion.identity).Init(_owner);

        yield return null;
    }

    // 기타 메서드
    public void TakeDamage(int damage)
    {
        if (_owner.MonsterData.CurHp <= 0 || _owner.IsOneFrameInvincible)
            return;

        _owner.IsOneFrameInvincible = true;
        _owner.HitFlash(_sr, _hitEffect);
        _owner.TakeDamage(damage);
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
}
