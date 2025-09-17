using UnityEngine;

/// <summary>
/// 대상을 추적하는데 사용하는 이동 방식
/// </summary>
public class ChaseMove : Game.Monster.IMovable
{
    private float _speed;
    Transform _tr;
    Rigidbody2D _rb;
    Collider2D _col;
    Transform _target;
    Animator _anim;

    // 임시 레이어
    // Todo: 벽과 땅에 대한 레이어가 생기면 이를 변경할 필요가 있음
    LayerMask _mask = ~(LayerMask.GetMask(Common.Layers.Player) | LayerMask.GetMask(Common.Layers.Monster));

    public ChaseMove(float speed, Transform tr, Rigidbody2D rb, Collider2D col, Transform target, Animator anim)
    {
        _speed = speed;
        _tr = tr;
        _rb = rb;
        _col = col;
        _target = target;
        _anim = anim;
    }

    public void Move()
    {
        if (_target == null)
            return;

        if (Mathf.Abs(_target.position.x - _tr.position.x) < 0.1f)
        {
            StopMove();
            return;
        }

        int dir = _target.position.x - _tr.position.x > 0 ? 1 : -1;

        _rb.linearVelocityX = _speed * dir;

        RaycastHit2D floor = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down, (_col.bounds.size.y / 2f + 0.1f), _mask);
        Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down * (_col.bounds.size.y / 2f + 0.1f), Color.red);
        RaycastHit2D wall = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x, 0.3f, _mask);
        Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x * 0.3f, Color.blue);

        if (floor.collider == null || wall.collider != null)
            StopMove();

        if(_rb.linearVelocityX != 0)
            _anim.SetFloat(Common.AnimatorParams.Speed, _speed);
    }

    public void StopMove()
    {
        _rb.linearVelocityX = 0;
        _anim.SetFloat(Common.AnimatorParams.Speed, 0);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
