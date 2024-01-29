using System;

public enum Mark
{
    None,
    Zero,
    Cross,
    Draw
}

public class TicTacToe
{
    private Mark[,] board = new Mark[3, 3];

    public Mark Turn { get; private set; } = Mark.Cross;

    public Mark this[int row, int col] => board[row, col];

    public TicTacToe Clone()
    {
        return new TicTacToe
        {
            board = (Mark[,])board.Clone(),
            Turn = Turn
        };
    }

    public void Play(int row, int col)
    {
        if (!CanPlay(row, col))
            throw new ArgumentException($"Cannot play on {row}, {col}.");

        board[row, col] = Turn;
        Turn = Turn == Mark.Cross ? Mark.Zero : Mark.Cross;
    }

    public bool CanPlay(int row, int col)
    {
        return row >= 0 &&
               row < board.GetLength(0) &&
               col >= 0 &&
               col < board.GetLength(1) &&
               board[row, col] == Mark.None;
    }

    public bool CanPlay()
    {
        for (var row = 0; row < 3; row++)
        for (var col = 0; col < 3; col++)
            if (board[row, col] == Mark.None)
                return true;

        return false;
    }

    public Mark Winner()
    {
        foreach (var mark in new[] { Mark.Zero, Mark.Cross })
        {
            for (var i = 0; i < 3; i++)
            {
                if (AllEqual(i, 0, 0, 1, mark)) return mark;
                if (AllEqual(0, i, 1, 0, mark)) return mark;
            }

            if (AllEqual(0, 0, 1, 1, mark)) return mark;
            if (AllEqual(0, 2, 1, -1, mark)) return mark;
        }

        if (!CanPlay()) return Mark.Draw;

        return Mark.None;
    }

    private bool AllEqual(int row, int col, int dr, int dc, Mark mark)
    {
        while (row >= 0 && row < board.GetLength(0) && col >= 0 && col < board.GetLength(1))
        {
            if (board[row, col] != mark)
                return false;

            row += dr;
            col += dc;
        }

        return true;
    }
}