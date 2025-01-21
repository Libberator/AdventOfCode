using System;
using System.Collections.Generic;

namespace AoC.Solutions.Y2021.D04;

public class Solution : ISolver
{
    private readonly List<BingoBoard> _boards = [];
    private int _bingoCount, _lastUnmarkedSum;
    private int[] _numbers = [];

    public void Setup(string[] input)
    {
        _numbers = Array.ConvertAll(input[0].Split(','), int.Parse);

        for (var i = 2; i < input.Length; i += 6)
        {
            var boardData = new int[5][];
            for (var j = 0; j < 5; j++)
                boardData[j] = Array.ConvertAll(input[i + j].Split(' ', StringSplitOptions.RemoveEmptyEntries),
                    int.Parse);
            var board = new BingoBoard(this, boardData);
            board.BingoCalled += OnBingoCalled;
            _boards.Add(board);
        }
    }

    public object SolvePart1()
    {
        StartNewGame();

        foreach (var number in _numbers)
        {
            CallNumber(number);
            if (_bingoCount > 0)
                return _lastUnmarkedSum * number;
        }

        return null!;
    }

    public object SolvePart2()
    {
        StartNewGame();

        foreach (var number in _numbers)
        {
            CallNumber(number);
            if (_bingoCount == _boards.Count)
                return _lastUnmarkedSum * number;
        }

        return null!;
    }

    private event Action NewGameStarted = delegate { };
    private event Action<int> NumberCalled = delegate { };

    private void StartNewGame()
    {
        _bingoCount = 0;
        NumberCalled = delegate { };
        NewGameStarted();
    }

    private void CallNumber(int number) => NumberCalled(number);

    private void OnBingoCalled(BingoBoard board)
    {
        _lastUnmarkedSum = board.GetUnmarkedSums();
        _bingoCount++;
    }

    private class BingoBoard
    {
        private readonly int[][] _board;
        private readonly Solution _source;
        private bool[,] _marked = new bool[5, 5];

        public BingoBoard(Solution source, int[][] newBoard)
        {
            _board = newBoard;
            _source = source;
            source.NewGameStarted += OnNewGameStarted;
        }

        public event Action<BingoBoard> BingoCalled = delegate { };

        private void OnNewGameStarted()
        {
            _marked = new bool[5, 5];
            ListenForNumbers();
        }

        private void ListenForNumbers() => _source.NumberCalled += OnNumberCalled;
        private void StopListening() => _source.NumberCalled -= OnNumberCalled;

        private void OnNumberCalled(int number)
        {
            if (!HasNumber(number, out var row, out var col) || !HasBingo(row, col)) return;
            BingoCalled(this);
            StopListening();
        }

        private bool HasNumber(int number, out int row, out int col)
        {
            row = col = -1;
            for (var i = 0; i < 5; i++)
                for (var j = 0; j < 5; j++)
                    if (_board[i][j] == number)
                    {
                        _marked[i, j] = true;
                        row = i;
                        col = j;
                        return true;
                    }

            return false;
        }

        private bool HasBingo(int row, int col) => CheckRow(row) || CheckCol(col);

        private bool CheckRow(int row)
        {
            for (var i = 0; i < 5; i++)
                if (!_marked[row, i])
                    return false;

            return true;
        }

        private bool CheckCol(int col)
        {
            for (var i = 0; i < 5; i++)
                if (!_marked[i, col])
                    return false;

            return true;
        }

        public int GetUnmarkedSums()
        {
            var sum = 0;
            for (var row = 0; row < 5; row++)
                for (var col = 0; col < 5; col++)
                    sum += !_marked[row, col] ? _board[row][col] : 0;

            return sum;
        }
    }
}