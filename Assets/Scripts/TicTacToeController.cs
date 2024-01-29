using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TicTacToeController : MonoBehaviour
{
    [SerializeField] private TicTacToeBlock[] row1;
    [SerializeField] private TicTacToeBlock[] row2;
    [SerializeField] private TicTacToeBlock[] row3;

    [SerializeField] private UnityEvent onVictory;
    [SerializeField] private UnityEvent onDefeatOrDraw;
    [SerializeField] private float aiThinkTime;
    [SerializeField] private float resetTime;

    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioSource audioSource;

    private TicTacToe _game;

    private void Awake()
    {
        _game = new TicTacToe();
    }

    public void Reset()
    {
        for (var i = 0; i < 3; i++)
        {
            row1[i].Reset();
            row2[i].Reset();
            row3[i].Reset();
        }

        _game = new TicTacToe();
    }

    private void MakeBlocksInteractable()
    {
        for (var i = 0; i < 3; i++)
        {
            row1[i].MakeInteractable();
            row2[i].MakeInteractable();
            row3[i].MakeInteractable();
        }
    }

    private void MakeBlocksUninteractable()
    {
        for (var i = 0; i < 3; i++)
        {
            row1[i].MakeUninteractable();
            row2[i].MakeUninteractable();
            row3[i].MakeUninteractable();
        }
    }

    public void PlayMove(TicTacToeBlock block)
    {
        StartCoroutine(PlayMoveCoroutine(block));
    }

    private IEnumerator PlayMoveCoroutine(TicTacToeBlock block)
    {
        block.PlayOn(true);
        MakeBlocksUninteractable();

        (int, int)? coords = null;

        for (var i = 0; i < 3; i++)
        {
            if (row1[i] == block)
                coords = (0, i);
            if (row2[i] == block)
                coords = (1, i);
            if (row3[i] == block)
                coords = (2, i);

            if (coords != null)
                break;
        }

        if (coords == null)
            throw new Exception("wtf");

        _game.Play(coords.Value.Item1, coords.Value.Item2);

        var gameEndingValue = _game.Winner() switch
        {
            Mark.Cross => 1,
            Mark.Draw => -1,
            Mark.Zero => -1,
            _ => 0
        };

        if (gameEndingValue > 0)
        {
            audioSource.PlayOneShot(victorySound);

            onVictory?.Invoke();
            yield break;
        }

        if (gameEndingValue < 0)
        {
            audioSource.PlayOneShot(failSound);

            onDefeatOrDraw?.Invoke();
            yield return new WaitForSeconds(resetTime);

            Reset();

            yield break;
        }

        yield return new WaitForSeconds(aiThinkTime);

        var aiMove = TicTacToeAI.BestMove(_game);

        switch (aiMove.Item1)
        {
            case 0:
                row1[aiMove.Item2].PlayOn(false);
                break;
            case 1:
                row2[aiMove.Item2].PlayOn(false);
                break;
            default:
                row3[aiMove.Item2].PlayOn(false);
                break;
        }

        _game.Play(aiMove.Item1, aiMove.Item2);

        gameEndingValue = _game.Winner() switch
        {
            Mark.Cross => 1,
            Mark.Draw => -1,
            Mark.Zero => -1,
            _ => 0
        };

        if (gameEndingValue < 0)
        {
            audioSource.PlayOneShot(failSound);

            onDefeatOrDraw?.Invoke();
            yield return new WaitForSeconds(resetTime);

            Reset();

            yield break;
        }

        MakeBlocksInteractable();
    }
}