using System.Collections;
using StarterAssets;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private const int PuzzlesCount = 2;

    [SerializeField] private Animator jailAnimator;
    [SerializeField] private Animator exitDoorAnimator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private TwoDMonkeyController monoController;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private AudioSource macacoSpectreSource;
    [SerializeField] private AudioSource entryDoorJailAudio;
    [SerializeField] private AudioSource exitDoorJailAudio;
    [SerializeField] private AudioSource[] entryJailsAudioSources;
    [SerializeField] private AudioClip[] stepClipVariations;
    [SerializeField] private AudioSource stepsAudioSource;
    [SerializeField] private float stepFrequency;
    [SerializeField] private AudioSource macacoEscapadoAudioSource;

    [SerializeField] private GameObject[] puzzleLights;
    [SerializeField] private string openJailTriggerName;
    [SerializeField] private string openExitDoorTriggerName;
    [SerializeField] private string playerStopMonogameTriggerName;

    private StarterAssetsInput _input;

    private int _puzzlesCompleted;

    private bool _wonTicTacToe;

    private void Awake()
    {
        _input = new StarterAssetsInput();
        _input.Enable();
        StartCoroutine(StepsCoroutine());
    }

    private IEnumerator StepsCoroutine()
    {
        while (true)
        {
            if (playerController.enabled && _input.Player.Move.ReadValue<Vector2>().magnitude > 0.1f)
            {
                var randomIndex = Random.Range(0, stepClipVariations.Length);
                stepsAudioSource.pitch = Random.Range(0.9f, 1.1f);
                stepsAudioSource.PlayOneShot(stepClipVariations[randomIndex]);
            }

            yield return new WaitForSeconds(stepFrequency);
        }
    }

    public void OnWinTicTacToe()
    {
        jailAnimator.SetTrigger(openJailTriggerName);
        foreach (var jailAudio in entryJailsAudioSources)
            jailAudio.Play();
    }

    public void OnClosedEntryJailDoor()
    {
        entryDoorJailAudio.Play();
    }

    public void OnWinPuzzle()
    {
        puzzleLights[_puzzlesCompleted].SetActive(true);

        _puzzlesCompleted++;

        if (_puzzlesCompleted >= PuzzlesCount)
        {
            exitDoorJailAudio.Play();
            exitDoorAnimator.SetTrigger(openExitDoorTriggerName);
        }
    }

    public void OnWinMonkeyGame()
    {
        playerAnimator.SetTrigger(playerStopMonogameTriggerName);
        monoController.enabled = false;

        macacoEscapadoAudioSource.Play();

        StartCoroutine(DisablePlayerAnimator());
    }

    private IEnumerator DisablePlayerAnimator()
    {
        yield return new WaitForSeconds(2.1f);
        playerAnimator.enabled = false;
    }

    public void TheEnd()
    {
        playerController.enabled = false;
        macacoSpectreSource.Play();
    }
}