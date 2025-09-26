using UnityEngine;

public class StartScene : MonoBehaviour
{
    [SerializeField]
    private Game.Player.PlayerCharacter _player;

    [SerializeField]
    private float _duration = 3f;

    [SerializeField]
    private Vector2 _direction = new Vector2(1, 0);

    void Start()
    {
        _player.AutoMove(_duration, _direction.normalized);
    }
}