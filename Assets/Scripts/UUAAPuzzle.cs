using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class UUAAPuzzle : MonoBehaviour
{
    [SerializeField] private ClipVariants[] sounds;
    [SerializeField] private int sequenceLength;
    [SerializeField] private Interactable[] leverInteractables;
    [SerializeField] private Interactable resetLeverInteractable;
    [SerializeField] private AudioSource puzzleAudioSource;
    [SerializeField] private float resetTime;

    [SerializeField] private GameObject waitForSequenceLabel;
    [SerializeField] private GameObject failLabel;
    [SerializeField] private GameObject useLeversLabel;
    [SerializeField] private GameObject waitToStartLabel;
    [SerializeField] private GameObject victoryLabel;

    [SerializeField] private UnityEvent onVictoryEvent;

    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip victorySound;

    private int _playedIndex;

    private int[] _targetSequence;

    private void GenerateSequence()
    {
        _playedIndex = 0;

        var maxNum = sounds.Length;

        _targetSequence = new int[sequenceLength];

        for (var i = 0; i < sequenceLength; i++)
            _targetSequence[i] = Random.Range(0, maxNum);
    }

    private IEnumerator PlaySequence()
    {
        foreach (var sound in _targetSequence)
            yield return PlayRandomVariant(sound);
    }

    private IEnumerator PlayRandomVariant(int sound)
    {
        var variants = sounds[sound].variants;
        var selected = variants[Random.Range(0, variants.Length)];

        puzzleAudioSource.PlayOneShot(selected);

        yield return new WaitForSeconds(selected.length);
    }

    public void StartPuzzle()
    {
        StopAllCoroutines();

        StartCoroutine(PuzzleCoroutine());
    }

    private IEnumerator PuzzleCoroutine()
    {
        GenerateSequence();

        failLabel.SetActive(false);
        useLeversLabel.SetActive(false);
        waitToStartLabel.SetActive(false);
        waitForSequenceLabel.SetActive(true);

        yield return new WaitForSeconds(1);

        yield return PlaySequence();

        waitForSequenceLabel.SetActive(false);
        useLeversLabel.SetActive(true);

        ActivateLevers();
    }

    private void ActivateLevers()
    {
        foreach (var lever in leverInteractables) lever.canInteract = true;
    }

    private void DeactivateLevers()
    {
        foreach (var lever in leverInteractables) lever.canInteract = false;
    }

    public void InteractLever(int index)
    {
        StartCoroutine(LeverInteractionCoroutine(index));
    }

    private IEnumerator LeverInteractionCoroutine(int lever)
    {
        leverInteractables[lever].canInteract = false;

        yield return PlayRandomVariant(lever);

        if (lever != _targetSequence[_playedIndex])
        {
            useLeversLabel.SetActive(false);
            failLabel.SetActive(true);
            DeactivateLevers();

            yield return new WaitForSeconds(1);

            puzzleAudioSource.PlayOneShot(failSound);

            yield return new WaitForSeconds(failSound.length);

            yield return new WaitForSeconds(resetTime);

            StartPuzzle();

            yield break;
        }

        _playedIndex++;

        if (_playedIndex == sequenceLength)
        {
            victoryLabel.SetActive(true);
            useLeversLabel.SetActive(false);
            resetLeverInteractable.canInteract = false;
            DeactivateLevers();

            yield return new WaitForSeconds(1);

            puzzleAudioSource.PlayOneShot(victorySound);

            yield return new WaitForSeconds(victorySound.length);

            onVictoryEvent?.Invoke();

            yield break;
        }

        leverInteractables[lever].canInteract = true;
    }
}

[Serializable]
public class ClipVariants
{
    public AudioClip[] variants;
}