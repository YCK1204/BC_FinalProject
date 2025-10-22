using UnityEngine;

public class BossZoneEntrance : MonoBehaviour
{
    [SerializeField] BossMonster Boss;
    [SerializeField] Transform BossSpawnPosition;

    private bool isTriggered = false;

    // 플레이어가 닿으면 보스 소환 및 연출(추가 예정)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer == (int)LayerMask.GetMask(Game.Monster.Layers.Player) && !isTriggered)
        {
            isTriggered = true;
            Instantiate(Boss, BossSpawnPosition.position, Quaternion.identity);
            Manager.Audio.SetBgm(AudioKey.BGM.BGM_BOSS_PHASE1);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blueViolet;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
#endif
}
