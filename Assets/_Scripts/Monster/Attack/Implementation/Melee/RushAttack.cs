using UnityEngine;

public class RushAttack : MeleeAttack
{
    // Todo: 돌진 속도 조절, 
    private Collider2D _hitBox;
    private Rigidbody2D _rb;

    private float _rushSpeed = 5f;

    public RushAttack(Transform tr, MonsterAttack monsterAttack) : base(tr, monsterAttack)
    {

    }

    // 사용할 콜라이더와 리지드 바디 설정, 원래는 생성자에서 하려고 했으나 호출 순서 문제로 함수로 빼서 나중에 호출
    public override void Init()
    {
        _hitBox = _monsterAttack.Owner.Col;
        _rb = _monsterAttack.Owner.Rb;

        _monsterAttack.OnAttackEnd += OnAttackEnd;
    }

    public override void Attack()
    {
        base.Attack();

        _rushSpeed = _monsterAttack.Owner.MonsterData.Speed * 2f;
        _monsterAttack.Owner.DeleteIgnoreCollider(_target);

        _rb.bodyType = RigidbodyType2D.Kinematic;
        _hitBox.isTrigger = true;

        float dir = _tr.localScale.x < 0 ? -1 : 1;
        _rb.linearVelocityX = dir * _rushSpeed;
    }

    public override void StopAttack()
    {
        OnAttackEnd();
    }

    private void OnAttackEnd()
    {
        if(_target != null)
            _monsterAttack.Owner.RegisterIgnoreCollider(_target);

        _rb.linearVelocityX = 0;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _hitBox.isTrigger = false;
    }

    public override bool GetCheckAttackable(float margin = 0)
    {
        float dir = _tr.localScale.x < 0 ? -1 : 1;
        float ySize = 0;

        if (_hitBox is BoxCollider2D)
            ySize = (_hitBox as BoxCollider2D).bounds.size.y;
        else if (_hitBox is CircleCollider2D)
            ySize = (_hitBox as CircleCollider2D).radius;

        _target = Physics2D.OverlapBox(_tr.position + new Vector3((_attackRange * 0.5f - margin) * dir, 0, 0), new Vector2(_attackRange, ySize), 0, _mask);

        return _target != null;
    }
}
