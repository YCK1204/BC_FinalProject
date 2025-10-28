using System;
using UnityEngine;

public abstract class BaseMonster : MonoBehaviour
{
    [SerializeField] protected MonsterDataHandler _dataHandler;
    public MonsterDataHandler MonsterData { get { return _dataHandler; } }

    protected Transform _target;
    public Transform Target { get { return _target; } }

    public int MonsterCount = 1;

    public Action OnDied;
    public Action OnDiedEnter;

    // 슈퍼아머 상태 확인 변수
    public bool IsSuperArmor = false;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void ResetTarget()
    {
        _target = null;
    }

    // 타겟을 바라보는 메서드
    public void LookTarget()
    {
        float d = _target.position.x < transform.position.x ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(d, transform.localScale.y, transform.localScale.z);
    }

    // 몬스터가 사망할 때 호출하는 메서드
    public virtual void Die()
    {
        OnDied?.Invoke();
        // Todo: 오브젝트 풀로 리턴
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    // 에디터에서 탐지 범위와 공격 가능 범위를 표시하는 메서드
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _dataHandler.DetectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _dataHandler.AttackRange);
    }
#endif
}

