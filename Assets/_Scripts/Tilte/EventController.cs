using UnityEngine;
using System.Collections;

public class EventController : MonoBehaviour
{
    [SerializeField]

    private Game.Player.PlayerCharacter _player;

    [SerializeField]
    private float _duration = 3f;

    [SerializeField]
    private Vector2 _direction = new Vector2(1, 0);

    [Header("Event")]
    public GameObject[] objects;

    private int currentIndex = 0;



    private IEnumerator Start()
    {
        _player.AutoMove(_duration, _direction.normalized);

        yield return new WaitForSeconds(3f);

        OnTriggerActive(0);
    }

    public void OnTriggerActive(int triggerIndex)
    {
        if (triggerIndex != currentIndex) return;

        if (currentIndex - 1 >= 0 && currentIndex - 1 < objects.Length)
        {
            Animator anim = objects[currentIndex - 1].GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("hide", true);
            }
        }

        if (currentIndex < objects.Length)
        {
            objects[currentIndex].SetActive(true);
        }

        currentIndex++;
    }
}
