using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2022.D01;

public class Solution : ISolver
{
    private readonly List<int> _data = [];
    public void Setup(string[] input) =>
        _data.AddRange(input.ChunkByNonEmpty().Select(c => c.ParseInts().Sum()));

    public object SolvePart1() => _data.Max();

    public object SolvePart2() => _data.OrderDescending().Take(3).Sum();
}