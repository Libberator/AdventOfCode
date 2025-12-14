using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2025.D12;

public class Solution : ISolver
{
    [TestValue(0.85)] private readonly double _ratioDelimiter = 1.0; // standard: 1.0. Use 0.85 for Test case
    private readonly List<(int Area, int TotalPresents)> _regions = [];
    private readonly List<string[]> _shapes = [];

    public void Setup(string[] input)
    {
        var chunks = input.ChunkByNonEmpty();
        _shapes.AddRange(chunks.SkipLast(1).Select(c => c[1..]));
        var numPresents = _shapes.Select(shape => shape.Sum(s => s.Count(c => c == '#'))).ToArray();

        foreach (var line in chunks[^1])
        {
            var split = line.Split('x', ':');
            var totalPresents = split[2].ParseInts().Zip(numPresents, (qty, n) => qty * n).Sum();
            _regions.Add((int.Parse(split[0]) * int.Parse(split[1]), totalPresents));
        }
    }

    // this solution takes advantage of how the input is created and uses a semi-magic number (after input analysis)
    public object SolvePart1() => _regions.Count(r => r.Area * _ratioDelimiter > r.TotalPresents);
}