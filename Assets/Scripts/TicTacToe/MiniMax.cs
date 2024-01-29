using System;
using System.Collections.Generic;
using System.Linq;

public class TicTacToeAI
{
    public static (int, int) BestMove(TicTacToe game)
    {
        var (row, col) = MiniMax(game, game.Turn);
        return (row, col);
    }

    private static (int, int) MiniMax(TicTacToe game, Mark player)
    {
        var moves = new List<(int, int, int)>();
        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
        {
            if (!game.CanPlay(i, j)) continue;
            int score;
            var nextGame = game.Clone();
            nextGame.Play(i, j);
            if (nextGame.Winner() == Mark.None)
                (score, _, _) = PlayMin(nextGame, player);
            else
                score = FinalScore(nextGame, player);
            moves.Add((score, i, j));
        }

        moves = moves.OrderByDescending(m => m.Item1).ToList();

        if (moves.Count == 1)
            return (moves[0].Item2, moves[0].Item3);

        var random = new Random();
        var index = random.Next(4) == 0 ? 1 : 0;
        return (moves[index].Item2,
            moves[index].Item3);
    }

    private static int FinalScore(TicTacToe game, Mark player)
    {
        var winner = game.Winner();

        if (winner == Mark.None)
            throw new InvalidOperationException("Game hasn't finished yet!");
        if (winner == player)
            return 1;
        if (winner == Mark.Draw)
            return 0;
        return -1;
    }

    private static (int, int, int) PlayMax(TicTacToe game, Mark player)
    {
        var bestScore = int.MinValue;
        var (bestRow, bestCol) = (-1, -1);

        for (var row = 0; row < 3; row++)
        for (var col = 0; col < 3; col++)
        {
            if (!game.CanPlay(row, col)) continue;

            var nextGame = game.Clone();
            nextGame.Play(row, col);

            int otherScore;

            if (nextGame.Winner() == Mark.None)
                (otherScore, _, _) = PlayMin(nextGame, player);
            else
                otherScore = FinalScore(nextGame, player);

            if (otherScore > bestScore)
            {
                bestScore = otherScore;
                bestRow = row;
                bestCol = col;
            }
        }

        return (bestScore, bestRow, bestCol);
    }

    private static (int, int, int) PlayMin(TicTacToe game, Mark player)
    {
        var bestScore = int.MaxValue;
        var (bestRow, bestCol) = (-1, -1);

        for (var row = 0; row < 3; row++)
        for (var col = 0; col < 3; col++)
        {
            if (!game.CanPlay(row, col)) continue;

            var nextGame = game.Clone();
            nextGame.Play(row, col);

            int otherScore;

            if (nextGame.Winner() == Mark.None)
                (otherScore, _, _) = PlayMax(nextGame, player);
            else
                otherScore = FinalScore(nextGame, player);

            if (otherScore < bestScore)
            {
                bestScore = otherScore;
                bestRow = row;
                bestCol = col;
            }
        }

        return (bestScore, bestRow, bestCol);
    }
}