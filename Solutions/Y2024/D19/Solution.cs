using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D19;

public class Solution : ISolver
{
    private readonly Dictionary<string, long> _cache = new() { [""] = 1 };
    private string[] _designs = [];
    private string[] _patterns = [];

    public void Setup(string[] input)
    {
        _patterns = input[0].Split(", ");
        _designs = input[2..];
    }

    public object SolvePart1() => _designs.Count(d => Permutations(d) > 0);

    public object SolvePart2() => _designs.Sum(Permutations);

    private long Permutations(string design) =>
        _cache.GetOrAdd(design, des => _cache[des] = _patterns.Where(des.StartsWith)
            .Sum(pattern => Permutations(des[pattern.Length..])));
}