using UnityEngine;
using Game.Player;
using System.Collections;

public class ShadowSlash : Skill
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Vector2 _spawnOffset = new Vector2(0.8f, 0.5f);
    [SerializeField] private int _count = 5;
    [SerializeField] private float _delay = 0.10f;

    public override void Execute()
    {
        StartCoroutine(ShootProjectilesDelay());
    }

    private IEnumerator ShootProjectilesDelay()
    {
        float facingSign = owner.StateMachine.FacingSign;
        Vector2 offset = new Vector2(_spawnOffset.x * facingSign, _spawnOffset.y);
        Vector2 spawnPosition;

        for (int i = 0; i < _count; i++)
        {
            spawnPosition = (Vector2)owner.transform.position + offset;
            Quaternion rotation = facingSign > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

            GameObject projectileObj = PlayerPool.Instance.GetFromPool(_projectilePrefab, spawnPosition, rotation);

            if (projectileObj.TryGetComponent(out ShadowSlashProjectile projectileInstance))
            {
                projectileInstance.Launch(owner);
            }

            if (i < _count - 1)
            {
                yield return new WaitForSeconds(_delay);
            }
        }
    }
}