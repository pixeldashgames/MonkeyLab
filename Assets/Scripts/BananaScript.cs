using UnityEngine;
using UnityEngine.Events;

public class BananaScript : MonoBehaviour
{
    [SerializeField] private UnityEvent onTouch;

    private void OnTriggerEnter2D(Collider2D other)
    {
        gameObject.SetActive(false);
        onTouch?.Invoke();
    }
}