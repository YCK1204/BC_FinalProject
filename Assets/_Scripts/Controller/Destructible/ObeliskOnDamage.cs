using Destructible2D;
using Game.Player;
using UnityEngine;

public class ObeliskOnDamage : ChildColliderController
{
    [SerializeField]
    protected float Force;
    public bool Splited = false;
    protected override void Start()
    {
        ImpactType = D2DImpactType.OnDamage;
        base.Start();
    }
    public override void HandleImpact()
    {
        if (Splited)
        {
            var playerTransform = PlayerCharacter.Instance.transform;

            Vector2 dir = transform.position - playerTransform.position;
            var dist = Vector2.Distance(transform.position, playerTransform.position);
            dir += (Vector2.up * Random.Range(0f, .5f));
            _rb2d.linearVelocity = (dir * Mathf.Clamp(dist, 1f, Force));
        }
    }
}
