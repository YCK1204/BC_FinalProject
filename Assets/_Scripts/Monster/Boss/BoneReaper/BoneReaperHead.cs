using Game.Monster;
using UnityEngine;

public class BoneReaperHead : MonoBehaviour, IDamageable
{
    private BoneReaper _owner;
    private Animator _anim;
    private BoxCollider2D _col;
    private Rigidbody2D _rb;

    private LayerMask _mask;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _col = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();

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
        target = Physics2D.OverlapBox(transform.position + AttackPos, new Vector2(lr, ud), 0, _mask);
#if UNITY_EDITOR
        Debug.DrawLine(transform.position + AttackPos - Vector3.right * lr + Vector3.up * ud, transform.position + AttackPos + Vector3.right * lr + Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + AttackPos - Vector3.right * lr - Vector3.up * ud, transform.position + AttackPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + AttackPos - Vector3.right * lr + Vector3.up * ud, transform.position + AttackPos - Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + AttackPos + Vector3.right * lr + Vector3.up * ud, transform.position + AttackPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
#endif

        if (target != null)
        {
            // Todo: 무적 체크

            float knockBackDirX = target.transform.position.x < AttackPos.x ? -1 : 1;
            Vector2 knockBackDir = new Vector2(knockBackDirX, 1);
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

    public void TakeDamage(int damage)
    {
        _owner.TakeDamage(damage);
    }
}
