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

    public void BreathAttackSmall()
    {

    }

    public void BreathAttackLarge()
    {

    }

    #endregion
    // 행동2: 오브 생성하기

    public void TakeDamage(int damage)
    {
        _owner.TakeDamage(damage);
    }
}
