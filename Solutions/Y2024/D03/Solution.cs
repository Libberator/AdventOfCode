using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC.Solutions.Y2024.D03;

public class Solution : ISolver
{
    private readonly List<(int Value, bool Enabled)> _products = [];

    public void Setup(string[] input)
    {
        var enabled = true;
        const string pattern = @"mul\((\d+),(\d+)\)|do(?:n't)?\(\)";

        foreach (var line in input)
            foreach (Match match in Regex.Matches(line, pattern))
                switch (match.Value)
                {
                    case "do()": enabled = true; break;
                    case "don't()": enabled = false; break;
                    default:
                        _products.Add((int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value),
                            enabled)); break;
                }
    }

    public object SolvePart1() => _products.Sum(p => p.Value);

    public object SolvePart2() => _products.Sum(p => p.Enabled ? p.Value : 0);
}