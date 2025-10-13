using Destructible2D;
using UnityEngine;

public enum D2DImpactType
{
    SplitStart,
    SplitEnd,
    OnDamage
}
public abstract class ChildColliderController : MonoBehaviour
{
    protected Collider2D _collider2d;
    protected Rigidbody2D _rb2d;

    protected D2DImpactType ImpactType;

    protected virtual void Start()
    {
        _collider2d = transform.parent.FindChild<Collider2D>(false, "Collision");
        _rb2d = transform.parent.GetComponent<Rigidbody2D>();

        switch (ImpactType)
        {
            case D2DImpactType.OnDamage:
                transform.parent.GetComponent<D2dDamage>().OnDamageChanged += HandleImpact;
                break;
            case D2DImpactType.SplitStart:
                transform.parent.GetComponent<D2dDestructibleSprite>().OnSplitStart += HandleImpact;
                break;
        }
    }
    public abstract void HandleImpact();
}