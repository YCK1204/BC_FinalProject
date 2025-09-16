using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    Transform _tr;
    Collider2D _col;
    Rigidbody2D _rb;

    float _maxCheckRayInterval = 0.2f;
    float _curCheckRayInterval = 0f;

    LayerMask _mask = ~(LayerMask.GetMask(Common.Layers.Player) | LayerMask.GetMask(Common.Layers.Monster));

    public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
        StateType = Common.StateType.Attack;

        _maxCheckRayInterval = 0.1f;
        _curCheckRayInterval = 0f;
    }

    public override void Enter()
    {
        base.Enter();

        _stateMachine.Owner.Anim.SetFloat(Common.AnimatorParams.Speed, 0);
        _stateMachine.Owner.Attack.OnAttackEnd += AttackEnd;

        _tr = _stateMachine.Owner.transform;
        _col = _stateMachine.Owner.Col;
        _rb = _stateMachine.Owner.Rb;

        Attack();
    }

    public override void Update()
    {
        base.Update();

        if(_curCheckRayInterval > _maxCheckRayInterval)
        {
            _curCheckRayInterval = 0;
            RaycastHit2D floor = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down, (_col.bounds.size.y / 2f + 0.1f), _mask);
            Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down * (_col.bounds.size.y / 2f + 0.1f), Color.red);
            RaycastHit2D wall = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x, 0.3f, _mask);
            Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x * 0.3f, Color.blue);

            if (floor.collider == null || wall.collider != null)
                _rb.linearVelocityX = 0;
        }
        _curCheckRayInterval += Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();

        _stateMachine.Owner.Attack.OnAttackEnd -= AttackEnd;
    }

    private void Attack()
    {
        _stateMachine.Owner.Anim.SetTrigger(Common.AnimatorParams.Attack);
    }

    private void AttackEnd()
    {
        if (CheckDetectRange())
        {
            // 감지가 되고 공격 범위 내라면 공격
            //if (CheckAttackRange())
            if(_stateMachine.Owner.Attack.Attackable.GetCheckAttackable())
            {
                Attack();
            }
            // 감지는 됬는데 공격 사정거리 밖이라면 추격
            else
            {
                _stateMachine.ChangeState(Common.StateType.Chase);
            }
        }
        else
        {
            _stateMachine.ChangeState(Common.StateType.Idle);
        }
    }
}
