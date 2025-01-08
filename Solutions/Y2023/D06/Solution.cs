using System;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2023.D06;

public class Solution : ISolver
{
    private long[] _distances = [];
    private int[] _times = [];

    public void Setup(string[] input)
    {
        _times = Utils.NumberPattern().Matches(input[0]).ParseMany<int>().ToArray();
        _distances = Utils.NumberPattern().Matches(input[1]).ParseMany<long>().ToArray();
    }

    public object SolvePart1() => Enumerable.Range(0, _times.Length)
        .Select(i => WaysToWin(_times[i], _distances[i])).Product();

    public object SolvePart2()
    {
        var time = _times.Aggregate((a, b) => a * b.NextPowerOf10() + b);
        var distance = _distances.Aggregate((a, b) => a * b.NextPowerOf10() + b);
        return WaysToWin(time, distance);
    }

    private static long WaysToWin(int time, long distance)
    {
        var result = Utils.BinarySearch(1L, time / 2, i => i * (time - i) > distance);
        return result >= 0 ? time - 2 * result + 1 : 0;
    }
}