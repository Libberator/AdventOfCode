using System;
using System.Collections.Generic;

namespace AoC.Solutions.Y2025.D11;

public class Solution : ISolver
{
    private const string P1Start = "you", End = "out";
    private const string P2Start = "svr", P2Mid1 = "dac", P2Mid2 = "fft";
    private readonly Dictionary<string, string[]> _mapping = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var from = line[..3];
            var to = line[4..].Split(' ', StringSplitOptions.TrimEntries);
            _mapping[from] = to;
        }
    }

    public object SolvePart1() => TotalPaths(P1Start, End, _mapping);

    public object SolvePart2()
    {
        var midCount = TotalPaths(P2Mid1, P2Mid2, _mapping);

        var startCount = TotalPaths(P2Start, midCount > 0 ? P2Mid1 : P2Mid2, _mapping);
        var endCount = TotalPaths(midCount > 0 ? P2Mid2 : P2Mid1, End, _mapping);
        midCount = midCount > 0 ? midCount : TotalPaths(P2Mid2, P2Mid1, _mapping);

        return startCount * midCount * endCount;
    }

    private static long TotalPaths(string from, string to, Dictionary<string, string[]> mapping)
    {
        Dictionary<string, long> memo = [];
        return Dfs(from);

        long Dfs(string current)
        {
            if (current == to)
                return 1;

            if (memo.TryGetValue(current, out var cached))
                return cached;

            long total = 0;

            if (mapping.TryGetValue(current, out var nextNodes))
                foreach (var next in nextNodes)
                    total += Dfs(next);

            memo[current] = total;
            return total;
        }
    }
}