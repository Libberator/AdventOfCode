using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AoC.Solutions.Y2023.D14;

public class Solution : ISolver
{
    private const char Cube = '#', Round = 'O', Empty = '.';
    private char[][] _grid = [];
    private static int _size;

    public void Setup(string[] input)
    {
        _grid = input.Select(line => line.ToCharArray()).ToArray();
        _size = input.Length;
    }

    public object SolvePart1()
    {
        TiltNorth();
        return GetScore(_grid);
    }

    public object SolvePart2()
    {
        const int cycles = 1_000_000_000;
        Dictionary<int, int> cache = [];
        List<int> scores = [];

        for (var i = 0; i < cycles; i++)
        {
            DoCycle();
            scores.Add(GetScore(_grid));
            var hash = GetHash(_grid);
            if (cache.TryAdd(hash, i))
                continue;

            var prevIteration = cache[hash];
            var period = i - prevIteration;
            var finalIteration = prevIteration + (cycles - i) % period - 1;

            return scores[finalIteration];
        }

        return GetScore(_grid);
    }

    private static int GetScore(char[][] grid) =>
        grid.Select((row, index) => (_size - index) * row.Count(c => c == Round)).Sum();

    private static int GetHash(char[][] grid)
    {
        var hash = 0;
        for (var row = 0; row < _size; row++)
            for (var col = 0; col < _size; col++)
                if (grid[row][col] == Round)
                    hash ^= row * _size + col;

        return hash;
    }

    private void DoCycle()
    {
        TiltNorth();
        TiltWest();
        TiltSouth();
        TiltEast();
    }

    private void TiltNorth() => Tilt(i => new ReadOnlySpan<char>(_grid.Select(row => row[i]).ToArray()),
        (i, from, to) => Move(from, i, to, i));

    private void TiltWest() => Tilt(i => new ReadOnlySpan<char>(_grid[i]),
        (i, from, to) => Move(i, from, i, to));

    private void TiltSouth() => Tilt(i => new ReadOnlySpan<char>(_grid.Select(row => row[i]).Reverse().ToArray()),
        (i, from, to) => Move(_size - from - 1, i, _size - to - 1, i));

    private void TiltEast() => Tilt(i => new ReadOnlySpan<char>(_grid[i].Reverse().ToArray()),
        (i, from, to) => Move(i, _size - from - 1, i, _size - to - 1));

    private void Move(int fromRow, int fromCol, int toRow, int toCol)
    {
        _grid[fromRow][fromCol] = Empty;
        _grid[toRow][toCol] = Round;
    }

    private static void Tilt(Func<int, ReadOnlySpan<char>> tileCreator, Action<int, int, int> moveAction)
    {
        Parallel.For(0, _size, i =>
        {
            var span = tileCreator(i);
            var nextCubeIndex = GetNextCubeIndex(span, -1);

            for (var to = 0; to < _size; to++)
            {
                if (to == nextCubeIndex)
                {
                    nextCubeIndex = GetNextCubeIndex(span, nextCubeIndex);
                    continue;
                }

                for (var from = to; from < nextCubeIndex; from++)
                {
                    if (span[from] != Round) continue;
                    moveAction(i, from, to);
                    to++;
                }

                to = nextCubeIndex - 1;
            }
        });
    }

    private static int GetNextCubeIndex(ReadOnlySpan<char> span, int currentCubeIndex)
    {
        var nextCubeIndexOffset = span[(currentCubeIndex + 1)..].IndexOf(Cube);
        return nextCubeIndexOffset == -1 ? _size : currentCubeIndex + 1 + nextCubeIndexOffset;
    }
}