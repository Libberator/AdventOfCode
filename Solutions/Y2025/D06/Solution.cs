using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2025.D06;

public class Solution : ISolver
{
    private List<IEnumerable<long>> _numbers = [];
    private char[] _operations = [];
    private List<IEnumerable<long>> _vertNumbers = [];

    public void Setup(string[] input)
    {
        _numbers = input[..^1].Select(Utils.ParseLongs).Transpose().ToList();
        _operations = input[^1].Where(c => c != ' ').ToArray();
        _vertNumbers = input[..^1].Transpose().ChunkByNonEmpty().Select(chunk => chunk.ParseLongs().AsEnumerable())
            .ToList();
    }

    public object SolvePart1() => GetTotal(_numbers, _operations);

    public object SolvePart2() => GetTotal(_vertNumbers, _operations);

    private static long GetTotal(List<IEnumerable<long>> numbers, char[] operations) =>
        operations.Select((op, i) => op switch
            {
                '*' => numbers[i].Product(),
                '+' => numbers[i].Sum(),
                _ => throw new ArgumentOutOfRangeException()
            })
            .Sum();
}