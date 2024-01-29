using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class TicTacToeBlock : MonoBehaviour
{
    [SerializeField] private Color selectedColor;
    [SerializeField] private GameObject crossOverlay;
    [SerializeField] private GameObject circleOverlay;
    [SerializeField] private TicTacToeController controller;

    private Color _defaultColor;
    private Interactable _interactable;
    private bool _played;
    private MeshRenderer _renderer;

    private void Awake()
    {
        _interactable = GetComponent<Interactable>();
        _renderer = GetComponent<MeshRenderer>();
        _defaultColor = _renderer.material.color;
    }

    public void Reset()
    {
        crossOverlay.SetActive(false);
        circleOverlay.SetActive(false);
        _played = false;
        _interactable.canInteract = true;
    }

    private void Update()
    {
        _renderer.material.color = _interactable.lookedAt ? selectedColor : _defaultColor;
    }

    public void MakeInteractable()
    {
        if (!_played)
            _interactable.canInteract = true;
    }

    public void MakeUninteractable()
    {
        _interactable.canInteract = false;
    }

    public void PlayOn(bool isCross)
    {
        _played = true;
        _interactable.canInteract = false;

        if (isCross)
            crossOverlay.SetActive(true);
        else
            circleOverlay.SetActive(true);
    }

    public void Interact()
    {
        controller.PlayMove(this);
    }
}