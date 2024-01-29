using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorPotionController : MonoBehaviour
{
    [SerializeField] private Color redColor;
    [SerializeField] private Color blueColor;
    [SerializeField] private Color greenColor;
    [SerializeField] private Color redBlueColor;
    [SerializeField] private Color redGreenColor;
    [SerializeField] private Color blueGreenColor;
    [SerializeField] private Color redGreenBlueColor;
    [SerializeField] private GameObject[] flaskPrefabs;
    [SerializeField] private Interactable resetLever;

    [SerializeField] private float resetTime;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip failClip;

    [SerializeField] private GameObject instructionsLabel;
    [SerializeField] private GameObject waitForStartLabel;
    [SerializeField] private GameObject victoryLabel;
    [SerializeField] private GameObject failLabel;
    [SerializeField] private Image colorSampleImage;

    [SerializeField] private GameObject[] currentFlasks;

    [SerializeField] private MeshRenderer potionMixRenderer;
    [SerializeField] private UnityEvent onVictory;

    private int[] _currentMix;
    private int _currentMixIndex;

    private Vector3[] _defaultFlaskPositions;

    private bool _mixing;
    private Color _targetColor;

    private void Awake()
    {
        _defaultFlaskPositions = new Vector3[currentFlasks.Length];
        for (var i = 0; i < currentFlasks.Length; i++) _defaultFlaskPositions[i] = currentFlasks[i].transform.position;
    }

    public void Reset()
    {
        foreach (var flask in currentFlasks)
            if (flask != null)
                Destroy(flask);

        for (var i = 0; i < flaskPrefabs.Length; i++)
            currentFlasks[i] = Instantiate(flaskPrefabs[i],
                _defaultFlaskPositions[i], flaskPrefabs[i].transform.rotation);

        _currentMix = new int[3];
        _currentMixIndex = 0;
        potionMixRenderer.material.color = GetCurrentColorMix();
        colorSampleImage.gameObject.SetActive(false);

        failLabel.SetActive(false);
        waitForStartLabel.SetActive(false);
    }

    public void StartGame()
    {
        Reset();

        colorSampleImage.gameObject.SetActive(true);
        instructionsLabel.SetActive(true);

        _mixing = true;

        _targetColor = Random.Range(0, 3) switch
        {
            0 => redGreenColor,
            1 => redBlueColor,
            _ => blueGreenColor
        };

        colorSampleImage.color = _targetColor;
    }

    public void OnDropFlask(int color)
    {
        StartCoroutine(OnDropFlaskCoroutine(color));
    }

    private IEnumerator OnDropFlaskCoroutine(int color)
    {
        if (!_mixing)
            yield break;

        _currentMix[_currentMixIndex] = color;
        _currentMixIndex++;
        var currentColor = GetCurrentColorMix();

        potionMixRenderer.material.color = currentColor;

        if (_currentMixIndex > 2)
        {
            _mixing = false;

            colorSampleImage.gameObject.SetActive(false);
            instructionsLabel.SetActive(false);
            failLabel.SetActive(true);

            audioSource.PlayOneShot(failClip);

            yield return new WaitForSeconds(resetTime);

            StartGame();

            yield break;
        }

        if (currentColor != _targetColor)
            yield break;

        colorSampleImage.gameObject.SetActive(false);
        instructionsLabel.SetActive(false);
        victoryLabel.SetActive(true);

        _mixing = false;

        resetLever.canInteract = false;

        audioSource.PlayOneShot(victoryClip);

        onVictory?.Invoke();
    }

    private Color GetCurrentColorMix()
    {
        return _currentMixIndex switch
        {
            0 => Color.white,
            1 => _currentMix[0] switch
            {
                0 => redColor,
                1 => greenColor,
                _ => blueColor
            },
            2 => _currentMix[0] switch
            {
                0 => _currentMix[1] switch
                {
                    1 => redGreenColor,
                    _ => redBlueColor
                },
                1 => _currentMix[1] switch
                {
                    0 => redGreenColor,
                    _ => blueGreenColor
                },
                _ => _currentMix[1] switch
                {
                    0 => redBlueColor,
                    _ => blueGreenColor
                }
            },
            _ => redGreenBlueColor
        };
    }
}