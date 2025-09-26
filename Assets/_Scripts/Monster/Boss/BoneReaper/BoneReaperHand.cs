using Game.Monster;
using UnityEngine;

public class BoneReaperHand : MonoBehaviour, IDamageable
{
    private BoneReaper _owner;
    private Animator _animator;
    private BoxCollider2D _col;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _col = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Init(BoneReaper boneReaper)
    {
        _owner = boneReaper;
    }

    // 행동1: 추적해서 내려찍기
    public void FollowTarget()
    {

    }

    public void Slam()
    {

    }
    // 행동2: 맵끝으로 가서 레이저 쏘기 시작해서 반대편에 도달하면 원래 위치로 돌아가기

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
}
