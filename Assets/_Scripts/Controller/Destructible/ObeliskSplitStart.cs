using Destructible2D;
using DG.Tweening;
using Game.Player;
using UnityEngine;

public class ObeliskSplitStart : ChildColliderController
{
    [SerializeField]
    protected float Force;
    [SerializeField]
    Transform GoldSpawnPoint;
    protected override void Start()
    {
        ImpactType = D2DImpactType.SplitStart;
        base.Start();
    }
    public override void HandleImpact()
    {
        _collider2d.isTrigger = true;
        _rb2d.constraints = RigidbodyConstraints2D.None;
        Manager.Resource.LoadAssetAsync<GoldController>("GoldUI", (go) =>
        {
            var gold = Instantiate(go);
            gold.transform.position = GoldSpawnPoint.position;
        });
    }
}
