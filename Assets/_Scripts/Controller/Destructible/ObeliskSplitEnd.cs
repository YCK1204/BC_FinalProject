using Destructible2D;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskSplitEnd : ChildColliderController
{
    public override void HandleImpact()
    {
        transform.parent.GetComponent<D2dDestructibleSprite>().OnSplitEnd += ResetFragmentDamage;
    }
    float _originalGravity;
    protected override void Start()
    {
        if (transform.parent.name.Contains("Clone"))
            return;
        ImpactType = D2DImpactType.SplitEnd;
        base.Start();
        _originalGravity = _rb2d.gravityScale;
        HandleImpact();
    }

    private void ResetFragmentDamage(List<D2dDestructible> fragments, D2dDestructible.SplitMode mode)
    {
        foreach (var fragment in fragments)
        {
            var rb = fragment.GetComponent<Rigidbody2D>();
            rb.gravityScale = _originalGravity;
            D2dDamage damageComponent = fragment.GetComponent<D2dDamage>();
            StartCoroutine(ImpactDelay(0.5f, fragment.transform.FindChild<ObeliskOnDamage>()));
            if (damageComponent != null)
                damageComponent.Damage = -999999f;
        }
    }
    IEnumerator ImpactDelay(float delay, ObeliskOnDamage onDmg)
    {
        yield return new WaitForSeconds(0.5f);
        onDmg.Splited = true;
    }
}
