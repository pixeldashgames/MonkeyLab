using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private bool selfDestruct;

    public void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke();

        if (selfDestruct)
            Destroy(gameObject);
    }
}