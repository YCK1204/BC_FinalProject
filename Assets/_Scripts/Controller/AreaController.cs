using Game.Player;
using System.Collections;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    ItemAreaData _data;
    Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>(); // 나중에 Area 데이터에 애니메이터 경로 넣고 설정해야함
    }
    public void Init(ItemAreaData data, PlayerCharacter owner)
    {
        _data = data;
    }
    IEnumerator CoShot()
    {
        float startTime = Time.time;
        while (Time.time - startTime < _data.AnmationDuration)
        {
            yield return null;
        }
        StopCoroutine(CoShot());
        Manager.Pool.Push<AreaController>(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 데미지 처리
        }
    }
}
