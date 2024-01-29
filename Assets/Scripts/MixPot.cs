using UnityEngine;

public class MixPot : MonoBehaviour
{
    [SerializeField] private ColorPotionController controller;

    private void OnTriggerEnter(Collider other)
    {
        var flask = other.GetComponent<Flask>();
        if (flask == null)
            return;

        controller.OnDropFlask(flask.colorIndex);

        Destroy(other.gameObject);
    }
}