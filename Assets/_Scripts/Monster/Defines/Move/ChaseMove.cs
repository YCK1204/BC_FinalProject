using UnityEngine;

public class ChaseMove : Game.Monster.IMovable
{
    private float _speed;
    Transform _tr;
    Rigidbody2D _rb;
    Collider2D _col;
    Transform _target;

    public ChaseMove(float speed, Transform tr, Rigidbody2D rb, Collider2D col, Transform target)
    {
        _speed = speed;
        _tr = tr;
        _rb = rb;
        _col = col;
        _target = target;
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

        RaycastHit2D floor = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down, (_col.bounds.size.y / 2f + 0.1f));
        Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down * (_col.bounds.size.y / 2f + 0.1f), Color.red);
        RaycastHit2D wall = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x, 0.3f);
        Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x * 0.3f, Color.blue);

        if (floor.collider == null || wall.collider != null)
            StopMove();
    }

    public void StopMove()
    {
        _rb.linearVelocityX = 0;
    }
}
