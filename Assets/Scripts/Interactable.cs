using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool oneShot;

    public bool canInteract = true;

    [HideInInspector] public bool lookedAt;

    [SerializeField] private UnityEvent onInteract;

    public void Interact()
    {
        if (!canInteract)
            return;

        if (oneShot)
            canInteract = false;

        onInteract?.Invoke();
    }
}