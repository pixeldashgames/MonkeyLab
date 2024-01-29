using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    private const int UIUpdatesPerSecond = 20;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private float interactDistance;

    private StarterAssetsInput _input;
    private Interactable _lookedInteractable;
    private bool _lookingAtInteractable;

    private void Awake()
    {
        _input = new StarterAssetsInput();
        _input.Enable();

        _input.Player.Interact.performed += InteractOnPerformed;

        StartCoroutine(UIUpdate());
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        if (!_lookingAtInteractable)
            return;

        _lookedInteractable.Interact();
    }

    private IEnumerator UIUpdate()
    {
        while (true)
        {
            var ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out var hitInfo, interactDistance))
            {
                var interactable = hitInfo.transform.GetComponent<Interactable>();

                if (interactable == null || !interactable.canInteract)
                {
                    if (_lookingAtInteractable)
                    {
                        interactPrompt.SetActive(false);
                        _lookingAtInteractable = false;
                        _lookedInteractable!.lookedAt = false;
                        _lookedInteractable = null;
                    }

                    yield return new WaitForSecondsRealtime(1f / UIUpdatesPerSecond);
                    continue;
                }

                if (!_lookingAtInteractable)
                {
                    _lookingAtInteractable = true;
                    interactPrompt.SetActive(true);
                    _lookedInteractable = interactable;
                    _lookedInteractable.lookedAt = true;
                }
                else if (interactable != _lookedInteractable)
                {
                    _lookedInteractable!.lookedAt = false;
                    _lookedInteractable = interactable;
                    _lookedInteractable.lookedAt = true;
                }
            }
            else if (_lookingAtInteractable)
            {
                interactPrompt.SetActive(false);
                _lookingAtInteractable = false;
                _lookedInteractable!.lookedAt = false;
                _lookedInteractable = null;
            }

            yield return new WaitForSecondsRealtime(1f / UIUpdatesPerSecond);
        }
    }
}