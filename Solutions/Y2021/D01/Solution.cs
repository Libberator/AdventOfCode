using System;

namespace AoC.Solutions.Y2021.D01;

public class Solution : ISolver
{
    private int[] _data = [];

    public void Setup(string[] input) => _data = Array.ConvertAll(input, int.Parse);

    public object SolvePart1() => GetIncreasedCount(1);

    public object SolvePart2() => GetIncreasedCount(3);

    private int GetIncreasedCount(int windowSize)
    {
        var increasedCount = 0;

        for (var i = windowSize; i < _data.Length; i++)
            if (_data[i] > _data[i - windowSize])
                increasedCount++;

        return increasedCount;
    }
}