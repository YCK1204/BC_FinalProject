using Game.Monster;
using Game.Player;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private BoneReaper _owner;
    private float _damage;
    private string _name;

    private float _offset = 3f;

    private LayerMask _mask;

    public void Init(BoneReaper owner, Vector3 position, string name)
    {
        _owner = owner;
        _damage = _owner.MonsterData.AttackPower;
        _name = name;

        transform.position = position;

        _mask = LayerMask.GetMask(Layers.Player);
    }

    public void VerticalBurst()
    {
        Manager.Audio.Play(AudioKey.Monster.Attack.M_ATK_BOSS_ORB_PHASE1, transform);

        Collider2D target;

        // 공중 0, 0에서 좌우 0.5 상하 3 박스
        Vector3 airAttakcPos = new Vector3(0, _offset * _owner.BossScale, 0);
        float lr = 0.5f * _owner.BossScale;
        float ud = 3f * _owner.BossScale;
        target = Physics2D.OverlapBox(transform.position + airAttakcPos, new Vector2(lr * 2, ud * 2), 0, _mask);
#if UNITY_EDITOR
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right * lr + Vector3.up * ud, transform.position + airAttakcPos + Vector3.right * lr + Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right * lr - Vector3.up * ud, transform.position + airAttakcPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right * lr + Vector3.up * ud, transform.position + airAttakcPos - Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos + Vector3.right * lr + Vector3.up * ud, transform.position + airAttakcPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
#endif

        if (target != null)
        {
            // 무적 체크
            PlayerCharacter pc = target.GetComponent<PlayerCharacter>();
            if (pc != null && pc.Invincible)
                return;

            float knockBackDirX = target.transform.position.x < airAttakcPos.x ? -1 : 1;
            Vector2 knockBackDir = new Vector2(knockBackDirX, 0.5f);
            knockBackDir.Normalize();

            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.linearVelocity = Vector2.zero;
                targetRb.AddForce(knockBackDir * 400);
            }

            target.GetComponent<IDamageable>()?.TakeDamage(_damage * 1.5f, _owner.gameObject);
            Debug.Log(_owner.MonsterData.AttackPower * 1.5f);
        }
    }

    public void SpikeBurst()
    {
        Collider2D target;

        // 지상 0, 0에서 좌우 1.5 상하 0.5 박스
        Vector3 airAttakcPos = new Vector3(0, _offset * _owner.BossScale, 0);
        float lr = 1.5f * _owner.BossScale;
        float ud = 0.5f * _owner.BossScale;
        target = Physics2D.OverlapBox(transform.position + airAttakcPos, new Vector2(lr * 2, ud * 2), 0, _mask);
#if UNITY_EDITOR
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right * lr + Vector3.up * ud, transform.position + airAttakcPos + Vector3.right * lr + Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right * lr - Vector3.up * ud, transform.position + airAttakcPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos - Vector3.right * lr + Vector3.up * ud, transform.position + airAttakcPos - Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
        Debug.DrawLine(transform.position + airAttakcPos + Vector3.right * lr + Vector3.up * ud, transform.position + airAttakcPos + Vector3.right * lr - Vector3.up * ud, Color.blue, 5f);
#endif

        if (target != null)
        {
            // 무적 체크
            PlayerCharacter pc = target.GetComponent<PlayerCharacter>();
            if (pc != null && pc.Invincible)
                return;

            float knockBackDirX = target.transform.position.x < airAttakcPos.x ? -1 : 1;
            Vector2 knockBackDir = new Vector2(knockBackDirX, 0.5f);
            knockBackDir.Normalize();

            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.linearVelocity = Vector2.zero;
                targetRb.AddForce(knockBackDir * 400);
            }

            target.GetComponent<IDamageable>()?.TakeDamage(_damage * 1.5f, _owner.gameObject);
            Debug.Log(_owner.MonsterData.AttackPower * 1.5f);
        }
    }

    public void Destroy()
    {
        _owner.OrbPool[_name].Push(gameObject);
    }

    public void OrbAttackEnd()
    {
        _owner.IsAttacking = false;
        this.Destroy();
    }
}
