using UnityEngine;

/// <summary>
/// 몬스터가 순찰하는데 사용하는 이동 방법
/// </summary>
public class PatrolMove : Game.Monster.IMovable
{
    StateMachineMonster _owner;
    private float _speed { get { return _owner.MonsterData.Speed; } }
    Transform _tr;
    Rigidbody2D _rb;
    Collider2D _col;

    // 레이어 마스크 그라운드
    // 임시 레이어
    // Todo: 벽과 땅에 대한 레이어가 생기면 이를 변경할 필요가 있음 -> 해결?
    LayerMask _mask = LayerMask.GetMask(Game.Monster.Layers.Ground) | LayerMask.GetMask(Game.Monster.Layers.N);

    public PatrolMove(StateMachineMonster owner)
    {
        _owner = owner;
        _tr = owner.transform;
        _rb = owner.Rb;
        _col = owner.Col;
    }

    public void Move()
    {
        int dir = _tr.localScale.x > 0 ? 1 : -1;
        _rb.linearVelocityX = _speed * dir;

        RaycastHit2D floor = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down, (_col.bounds.size.y + 0.1f), _mask);
        Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.down * (_col.bounds.size.y + 0.1f), Color.red);
        RaycastHit2D wall = Physics2D.Raycast(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x, 0.3f, _mask);
        Debug.DrawRay(_tr.position + new Vector3(_col.bounds.size.x / 2f + 0.1f, 0, 0) * _tr.localScale.x, Vector2.right * _tr.localScale.x * 0.3f, Color.blue);

        if(floor.collider == null || wall.collider != null)
            _tr.localScale = new Vector3(-1 * _tr.localScale.x, _tr.localScale.y, _tr.localScale.z);

    }

    public void StopMove()
    {
        _rb.linearVelocityX = 0;
    }
}
