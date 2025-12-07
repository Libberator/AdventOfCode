using System.Collections.Generic;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2025.D07;

public class Solution : ISolver
{
    private const char Start = 'S', Splitter = '^';
    private static string[] _input = [];
    private Vec2D _startPos;

    public void Setup(string[] input)
    {
        _input = input;
        _startPos = input.FindPosOf(Start);
    }

    public object SolvePart1()
    {
        var splits = 0;
        _ = Step(_startPos, [], ref splits);
        return splits;
    }

    public object SolvePart2()
    {
        var _ = 0;
        return Step(_startPos, [], ref _);
    }

    private static long Step(Vec2D current, Dictionary<Vec2D, long> memo, ref int splits)
    {
        if (memo.TryGetValue(current, out var paths))
            return paths;

        var next = current + Vec2D.S;

        if (next.X >= _input.Length)
            return memo[current] = 1;
        if (_input.GetAt(next) != Splitter)
            return memo[current] = Step(next, memo, ref splits);

        splits++;
        return memo[current] = Step(next + Vec2D.W, memo, ref splits) + Step(next + Vec2D.E, memo, ref splits);
    }
}