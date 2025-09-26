using Game.Monster;
using UnityEngine;

public class BoneReaperHead : MonoBehaviour, IDamageable
{
    private BoneReaper _owner;
    private Animator _animator;
    private BoxCollider2D _col;
    private Rigidbody2D _rb;

    public void Init(BoneReaper boneReaper)
    {
        _owner = boneReaper;
    }

    // 행동1: 브레스 뿜기 공격 판정
    // 행동2: 오브 생성하기

    public void TakeDamage(int damage)
    {
        _owner.TakeDamage(damage);
    }
}
