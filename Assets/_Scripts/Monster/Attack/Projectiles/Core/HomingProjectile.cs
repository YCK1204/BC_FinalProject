using System.Collections;
using UnityEngine;

public class HomingProjectile : BaseProjectile
{
    private Coroutine _rotateCoroutine;

    private void OnEnable()
    {
        transform.localEulerAngles = Vector3.zero;
    }

    public override void Init(Vector3 dir, Transform target = null)
    {
        base.Init(dir, target);

        if (_target != null && _rotateCoroutine == null)
        {
            _rotateCoroutine = StartCoroutine(Rotate());
        }
    }

    protected override void Move()
    {
        _rb.linearVelocity = transform.right * _dir.x * Data.Speed;
    }

    private IEnumerator Rotate()
    {
        while (_target != null)
        {
            Vector2 dir = ((Vector2)_target.position - _rb.position).normalized;

            // 목표 회전값
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // 현재 회전에서 목표 회전으로 부드럽게 회전
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                Data.RotateSpeed * Time.deltaTime
            );

            yield return null;
        }
    }
}
