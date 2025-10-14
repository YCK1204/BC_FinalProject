using UnityEngine;
using Game.Player;

public class ShadowSlash : Skill
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Vector2 _spawnOffset = new Vector2(0.8f, 0.5f);

    public override void Execute()
    {
        //if (_projectilePrefab == null)
        //{
        //    Debug.LogError("cc", this.gameObject);
        //    return;
        //}

        float facingSign = owner.StateMachine.FacingSign;
        Vector2 offset = new Vector2(_spawnOffset.x * facingSign, _spawnOffset.y);
        Vector2 spawnPosition = (Vector2)owner.transform.position + offset;
        Quaternion rotation = facingSign > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        GameObject projectileObj = PlayerPool.Instance.GetFromPool(_projectilePrefab, spawnPosition, rotation);

        if (projectileObj.TryGetComponent(out ShadowSlashProjectile projectileInstance))
        {
            projectileInstance.Launch(owner);
        }
    }
}