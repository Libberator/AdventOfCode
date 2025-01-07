using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D07;

public class Solution : ISolver
{
    private readonly List<(long target, long[] values)> _input = [];

    public void Setup(string[] input)
    {
        foreach (var split in input.Select(l => l.Split(": ")))
            _input.Add((long.Parse(split[0]), split[1].ParseLongs()));
    }

    public object SolvePart1() => _input
        .Where(i => IsValid(i.target, i.values))
        .Sum(i => i.target);

    public object SolvePart2() => _input
        .Where(i => IsValid(i.target, i.values, useConcat: true))
        .Sum(i => i.target);

    private static bool IsValid(long target, long[] values, int index = 0, long total = 0, bool useConcat = false)
    {
        if (total > target) return false;
        if (index == values.Length) return total == target;

        return (useConcat && IsValid(target, values, index + 1, Concat(total, values[index]), useConcat)) ||
               IsValid(target, values, index + 1, total * values[index], useConcat) ||
               IsValid(target, values, index + 1, total + values[index], useConcat);
    }

    private static long Concat(long a, long b)
    {
        var powerOf10 = 10;
        while (powerOf10 <= b)
            powerOf10 *= 10;
        return a * powerOf10 + b;
    }
}