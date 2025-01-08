using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D21;

public class Solution : ISolver
{
    private const char Empty = ' ';
    private readonly string[] _arrowKeys = [$"{Empty}^A", "<v>"];
    private readonly Dictionary<(char, char, int), long> _cache = [];
    private readonly string[] _numPad = ["789", "456", "123", $"{Empty}0A"];
    private string[] _data = [];

    public void Setup(string[] input) => _data = input;

    public object SolvePart1() => _data.Sum(line => ShortestPath(_numPad, line, 2) * GetValue(line));

    public object SolvePart2() => _data.Sum(line => ShortestPath(_numPad, line, 25) * GetValue(line));

    private static long GetValue(string line) => long.Parse(line[..3]);

    private long ShortestPath(string[] grid, string line, int depth)
    {
        long total = 0;
        line = 'A' + line;
        for (var i = 0; i < line.Length - 1; i++)
            total += ShortestPath(grid, line[i], line[i + 1], depth);
        return total;
    }

    private long ShortestPath(string[] grid, char start, char end, int depth)
    {
        if (_cache.TryGetValue((start, end, depth), out var score))
            return score;

        var startPos = grid.FindPosOf(start);
        var endPos = grid.FindPosOf(end);

        if (depth == 0)
        {
            score = startPos.DistanceManhattan(endPos) + 1;
            _cache.Add((start, end, depth), score);
            return score;
        }

        var diff = endPos - startPos;
        var hor = (diff.Y > 0 ? '>' : '<').Repeat(Math.Abs(diff.Y));
        var ver = (diff.X > 0 ? 'v' : '^').Repeat(Math.Abs(diff.X));
        var a = $"{hor}{ver}A";
        var b = $"{ver}{hor}A";

        var emptyPos = grid.FindPosOf(Empty);
        var cannotGoHorFirst = startPos.X == emptyPos.X && endPos.Y == emptyPos.Y;
        var cannotGoVerFirst = startPos.Y == emptyPos.Y && endPos.X == emptyPos.X;

        score = cannotGoHorFirst || a == b ? ShortestPath(_arrowKeys, b, depth - 1) :
            cannotGoVerFirst ? ShortestPath(_arrowKeys, a, depth - 1) :
            Math.Min(ShortestPath(_arrowKeys, b, depth - 1), ShortestPath(_arrowKeys, a, depth - 1));

        _cache.Add((start, end, depth), score);
        return score;
    }
}