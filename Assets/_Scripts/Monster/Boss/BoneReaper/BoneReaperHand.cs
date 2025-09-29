using Game.Monster;
using Game.Player;
using System.Collections;
using UnityEngine;

public class BoneReaperHand : MonoBehaviour, IDamageable
{
    private BoneReaper _owner;
    private Animator _anim;
    public Animator Anim {  get { return _anim; } }
    private BoxCollider2D _col;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    private Coroutine _hitEffect;

    private Vector3 _originPos;
    private float _offset = 1.33f;
    private float _followYOffeset = 3f;

    private float _curFollowTime = 0f;
    private float _maxFollowTime = 3f;
    private float _speed = 3f;

    private LayerMask _mask;

    [Header("레이저 공격 시작 및 종료 위치")]
    private Vector3 _leftStartLocalPos = new Vector3(-8f, 0, 0);
    private Vector3 _leftEndLocalPos = new Vector3(10f, 0, 0);
    private Vector3 _rightStartLocalPos = new Vector3(8f, 0, 0);
    private Vector3 _rightEndLocalPos = new Vector3(-10f, 0, 0);

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _col = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        _originPos = transform.position;
        _mask = LayerMask.GetMask(Layers.Player);
    }

    public void Init(BoneReaper boneReaper)
    {
        _owner = boneReaper;
    }

    #region Slam
    // 행동1: 추적해서 내려찍기
    public void Slam(float delay = 0f)
    {
        StartCoroutine(FollowTarget(delay));
    }

    public void SlamAttack()
    {
        transform.position -= Vector3.up * _followYOffeset;
        // 공중 공격 판정 확인 이후 없으면 지면 판정도 확인
        Collider2D target;
        float dir = transform.localScale.x < 0 ? -1 : 1;

        float aLR = 1f;
        float aUD = 1.5f + _followYOffeset/2;

        // 공중 -1.25(@),1.5 에서 좌우 1 상하 1.5 박스
        Vector3 airAttakcPos = new Vector3(-1.25f * dir, 1.5f + _followYOffeset / 2f, 0);
        target = Physics2D.OverlapBox(transform.position + airAttakcPos, new Vector2(aLR * 2, aUD * 2), 0, _mask);
#if UNITY_EDITOR
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right + Vector3.up * aUD, transform.position + airAttakcPos + Vector3.right + Vector3.up * aUD, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right - Vector3.up * aUD, transform.position + airAttakcPos + Vector3.right - Vector3.up * aUD, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right + Vector3.up * aUD, transform.position + airAttakcPos - Vector3.right - Vector3.up * aUD, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos + Vector3.right + Vector3.up * aUD, transform.position + airAttakcPos + Vector3.right - Vector3.up * aUD, Color.blue, 5f);
#endif

        // 지면 -1(@),0.5 에서 좌우 2.5 상하 0.5f 박스
        Vector3 groundAttakcPos = new Vector3(-1 * dir, 0.5f, 0);
        if(target == null)
        {
            target = Physics2D.OverlapBox(transform.position + groundAttakcPos, new Vector2(2.5f * 2, 0.5f * 2), 0, _mask);
#if UNITY_EDITOR
            Debug.DrawLine(transform.position + groundAttakcPos - Vector3.right * 2.5f + Vector3.up * 0.5f, transform.position + groundAttakcPos + Vector3.right * 2.5f + Vector3.up * 0.5f, Color.red, 5f);
            Debug.DrawLine(transform.position + groundAttakcPos - Vector3.right * 2.5f - Vector3.up * 0.5f, transform.position + groundAttakcPos + Vector3.right * 2.5f - Vector3.up * 0.5f, Color.red, 5f);
            Debug.DrawLine(transform.position + groundAttakcPos - Vector3.right * 2.5f + Vector3.up * 0.5f, transform.position + groundAttakcPos - Vector3.right * 2.5f - Vector3.up * 0.5f, Color.red, 5f);
            Debug.DrawLine(transform.position + groundAttakcPos + Vector3.right * 2.5f + Vector3.up * 0.5f, transform.position + groundAttakcPos + Vector3.right * 2.5f - Vector3.up * 0.5f, Color.red, 5f);
#endif
        }

        if(target != null)
        {
            // 무적 체크
            PlayerCharacter pc = target.GetComponent<PlayerCharacter>();
            if (pc != null && pc.Invincible)
                return;

            float knockBackDirX = target.transform.position.x < groundAttakcPos.x ? -1 : 1;
            Vector2 knockBackDir = new Vector2(knockBackDirX, 0.5f);
            knockBackDir.Normalize();

            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if(targetRb != null)
            {
                targetRb.linearVelocity = Vector2.zero;
                targetRb.AddForce(knockBackDir * 400);
            }

            target.GetComponent<IDamageable>()?.TakeDamage(_owner.MonsterData.AttackPower);
            Debug.Log(_owner.MonsterData.AttackPower);
        }
    }

    private IEnumerator FollowTarget(float delay)
    {
        if(delay != 0f)
            yield return new WaitForSeconds(delay);

        float targetX = 0;
        float curHeight = transform.position.y;
        while (_curFollowTime < _maxFollowTime)
        {
            float dir = transform.localScale.x < 0 ? -1 : 1;
            targetX = _owner.Target.position.x + _offset * dir;
            if (Mathf.Abs(transform.position.x - targetX) > 0.3f)
            {
                Vector3 pos = transform.position;
                Vector3 newPos = Vector3.Lerp(pos, new Vector3(targetX, curHeight + _followYOffeset, transform.position.z), Time.deltaTime * _speed);
                float newX = Mathf.Lerp(pos.x, targetX, Time.deltaTime * _speed);
                //transform.position = new Vector3(newX, pos.y, pos.z);
                transform.position = newPos;
                _curFollowTime += Time.deltaTime;
                yield return null;
            }
            else
                break;
        }
        _curFollowTime = 0;
        transform.position = new Vector3(targetX, curHeight + _followYOffeset, transform.position.z);
        _anim.SetTrigger(BoneReaperAnimatorParams.Slam);
        _owner.Head.TriggerAnimation(BoneReaperAnimatorParams.Action);
        yield return null;
    }
    #endregion

    #region Laser
    // 행동2: 맵끝으로 가서 레이저 쏘기 시작해서 반대편에 도달하면 원래 위치로 돌아가기
    public void Laser()
    {
        StartCoroutine(MoveToStartPos());
    }
    
    private IEnumerator MoveToStartPos()
    {
        Vector3 startLocalPos = transform.localScale.x < 0 ? _rightStartLocalPos : _leftStartLocalPos;
        while (true)
        {
            if (Mathf.Abs(transform.localPosition.x - startLocalPos.x) > 0.1f)
            {
                Vector3 pos = transform.localPosition;
                float newX = Mathf.Lerp(pos.x, startLocalPos.x, Time.deltaTime * _speed);
                transform.localPosition = new Vector3(newX, pos.y, pos.z);
                yield return null;
            }
            else
                break;
        }
        transform.localPosition = startLocalPos;
        _anim.SetBool(BoneReaperAnimatorParams.IsLaserStart, true);
        _owner.Head.SetBoolAnimation(BoneReaperAnimatorParams.IsLaserStart, true);
        StartCoroutine(MoveToEndPos());
        yield return null; 
    }

    public void LaserAttack()
    {
        Collider2D target;
        float dir = transform.localScale.x < 0 ? -1 : 1;

        // 공중 -1.4(@), 2 에서 좌우 0.5 상하 2 박스
        Vector3 airAttakcPos = new Vector3(-1.4f * dir, 2f, 0);
        float lr = 0.5f;
        float ud = 2f;
        target = Physics2D.OverlapBox(transform.position + airAttakcPos, new Vector2(lr * 2, ud * 2), 0, _mask);
#if UNITY_EDITOR
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right * lr + Vector3.up * ud, transform.position + airAttakcPos + Vector3.right * lr + Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right * lr - Vector3.up * ud, transform.position + airAttakcPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right * lr + Vector3.up * ud, transform.position + airAttakcPos - Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos + Vector3.right * lr + Vector3.up * ud, transform.position + airAttakcPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
#endif

        if (target != null)
        {
            // 무적 체크
            PlayerCharacter pc = target.GetComponent<PlayerCharacter>();
            if (pc != null && pc.Invincible)
                return;

            float knockBackDirX = target.transform.position.x < airAttakcPos.x ? -1 : 1;
            Vector2 knockBackDir = new Vector2(knockBackDirX, 0.5f);
            knockBackDir.Normalize();

            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.linearVelocity = Vector2.zero;
                targetRb.AddForce(knockBackDir * 400);
            }

            target.GetComponent<IDamageable>()?.TakeDamage(_owner.MonsterData.AttackPower * 1.5f);
            Debug.Log((int)(_owner.MonsterData.AttackPower * 1.5f));
        }
    }

    public IEnumerator MoveToEndPos()
    {
        // 왼쪽은 로컬 기준 -8에서 8.5까지
        // 오른쪽은 로컬 기준 6에서 -10.5까지
        Vector3 startLocalPos = transform.localScale.x < 0 ? _rightStartLocalPos : _leftStartLocalPos;
        Vector3 endLocalPos = transform.localScale.x < 0 ? _rightEndLocalPos : _leftEndLocalPos;

        while (true)
        {
            if (Mathf.Abs(transform.localPosition.x - endLocalPos.x) > 0.1f)
            {
                Vector3 pos = transform.localPosition;
                float newX = Mathf.MoveTowards(pos.x, endLocalPos.x, Time.deltaTime * _speed);
                transform.localPosition = new Vector3(newX, pos.y, pos.z);
                yield return null;
            }
            else
                break;
        }
        transform.localPosition = endLocalPos;
        _anim.SetBool(BoneReaperAnimatorParams.IsLaserStart, false);
        _owner.Head.SetBoolAnimation(BoneReaperAnimatorParams.IsLaserStart, false);

        yield return null;
    }
    #endregion

    // 원래 위치로 복귀하는 코드
    public IEnumerator Return()
    {
        while (true)
        {
            if (Mathf.Abs(transform.position.x - _originPos.x) > 0.1f)
            {
                Vector3 pos = transform.position;
                float newX = Mathf.Lerp(pos.x, _originPos.x, Time.deltaTime * _speed);
                transform.position = new Vector3(newX, pos.y, pos.z);
                yield return null;
            }
            else
                break;
        }
        transform.position = _originPos;

        // 슬램 카운트가 남아있으면 
        if(_owner._curRunningHandsCount > 0)
            _owner._curRunningHandsCount--;
        if(_owner._curRunningHandsCount <= 0)
            _owner.IsAttacking = false;
        yield return null;
    }

    public IEnumerator DelayAnimation(float delay)
    {
        _anim.speed = 0;
        yield return new WaitForSeconds(delay);
        _anim.speed = 1;
    }
    // 그 외 메서드

    public void TakeDamage(float damage)
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
}
