using System;
using UnityEngine;

public enum InteractableType
{
    Item,
    Gold,
    NPC,
    Portal
}
public abstract class InteractableController : MonoBehaviour
{
    protected Action onEnterTrigger;
    protected Action onExitTrigger;
    public InteractableType Type { get; protected set; }
    void Start()
    {
        Init();
    }
    protected virtual void Init()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            onEnterTrigger();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            onExitTrigger();
    }
    public abstract void OnInteract();

}
