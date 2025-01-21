using System;
using System.Linq;

namespace AoC.Solutions.Y2021.D06;

public class Solution : ISolver
{
    private readonly long[] _data = new long[9];

    public void Setup(string[] input)
    {
        var digits = Array.ConvertAll(input[0].Split(','), int.Parse);
        foreach (var digit in digits) _data[digit]++;
    }

    public object SolvePart1() => GetTotalAfterNDays(_data, 80);

    public object SolvePart2() => GetTotalAfterNDays(_data, 256);

    private static long GetTotalAfterNDays(long[] source, int totalDays)
    {
        var copy = new long[9];
        Array.Copy(source, copy, copy.Length);

        for (var i = 0; i < totalDays; i++)
            copy[(i + 7) % 9] += copy[i % 9];
        return copy.Sum();
    }
}