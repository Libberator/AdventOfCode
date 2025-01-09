using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2023.D09;

public class Solution : ISolver
{
    private readonly List<int[]> _data = [];

    public void Setup(string[] input) => _data.AddRange(input.Select(Utils.ParseInts));

    public object SolvePart1() => _data.Sum(NextValueInPattern);

    public object SolvePart2() => _data.Sum(PrevValueInPattern);

    private static int NextValueInPattern(int[] pattern)
    {
        var diffStack = GetDiffStack(pattern);
        var nextValue = 0;

        while (diffStack.Count > 0)
            nextValue += diffStack.Pop()[^1];

        return nextValue;
    }

    private static int PrevValueInPattern(int[] pattern)
    {
        var diffStack = GetDiffStack(pattern);
        var prevValue = 0;

        while (diffStack.Count > 0)
            prevValue = diffStack.Pop()[0] - prevValue;

        return prevValue;
    }

    private static Stack<int[]> GetDiffStack(int[] pattern)
    {
        Stack<int[]> diffStack = [];

        while (pattern.Any(d => d != 0))
        {
            diffStack.Push(pattern);
            pattern = GetDiff(pattern);
        }

        return diffStack;
    }

    private static int[] GetDiff(int[] pattern)
    {
        var diff = new int[pattern.Length - 1];

        for (var i = 0; i < diff.Length; i++)
            diff[i] = pattern[i + 1] - pattern[i];

        return diff;
    }
}