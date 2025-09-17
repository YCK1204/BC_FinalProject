using UnityEngine;

public class NormalProjectile : BaseProjectile
{
    protected override void Move()
    {
        _rb.linearVelocityX = _dir.x * Data.Speed;
    }
}
