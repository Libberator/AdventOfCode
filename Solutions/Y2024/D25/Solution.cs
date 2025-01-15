using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D25;

public class Solution : ISolver
{
    private const int Height = 7;
    private readonly List<int[]> _keys = [];
    private readonly List<int[]> _locks = [];

    public void Setup(string[] input)
    {
        var chunks = input.ChunkByNonEmpty();

        foreach (var chunk in chunks)
            if (chunk[0][0] == '#') _locks.Add(Parse(chunk));
            else _keys.Add(Parse(chunk));
    }

    public object SolvePart1() => _keys.Sum(k => _locks.Count(l => Fits(k, l)));

    private static bool Fits(int[] k, int[] l)
    {
        for (var i = 0; i < k.Length; i++)
            if (k[i] + l[i] > Height)
                return false;
        return true;
    }

    private static int[] Parse(string[] chunk)
    {
        var result = new int[chunk[0].Length];
        foreach (var line in chunk)
            for (var i = 0; i < line.Length; i++)
                result[i] += line[i] == '#' ? 1 : 0;

        return result;
    }
}