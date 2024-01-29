using UnityEngine;

public class SpikeDeathScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] bananas;

    private Vector3 _defaultPlayerPosition;

    private void Awake()
    {
        _defaultPlayerPosition = player.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        player.position = _defaultPlayerPosition;
        foreach (var banana in bananas)
            banana.SetActive(true);
    }
}