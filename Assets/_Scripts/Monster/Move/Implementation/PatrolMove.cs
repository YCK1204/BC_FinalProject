using UnityEngine;

/// <summary>
/// 몬스터가 순찰하는데 사용하는 이동 방법
/// </summary>
public class PatrolMove : Game.Monster.IMovable
{
    private float _speed;
    Transform _tr;
    Rigidbody2D _rb;
    Collider2D _col;

    // 레이어 마스크 그라운드
    // 레이어 마스크 벽

    public PatrolMove(float speed, Transform tr, Rigidbody2D rb, Collider2D col)
    {
        _speed = speed;
        _tr = tr;
        _rb = rb;
        _col = col;
    }

    public void Move()
    {
        _rb.linearVelocityX = _speed * _tr.localScale.x;
        RaycastHit2D floor = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down, (_col.bounds.size.y / 2f + 0.1f));
        Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down * (_col.bounds.size.y / 2f + 0.1f), Color.red);
        RaycastHit2D wall = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x, 0.3f);
        Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x * 0.3f, Color.blue);

        if(floor.collider == null || wall.collider != null)
            _tr.localScale = new Vector3( -1 * _tr.localScale.x, _tr.localScale.y, _tr.localScale.z);
    }

    public void StopMove()
    {
        _rb.linearVelocityX = 0;
    }
}
