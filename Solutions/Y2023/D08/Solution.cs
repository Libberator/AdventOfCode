using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2023.D08;

public class Solution : ISolver
{
    private const string Start = "AAA", End = "ZZZ";
    private readonly Dictionary<string, string[]> _stepsMap = [];
    private string _directions = "";

    public void Setup(string[] input)
    {
        _directions = input[0];
        foreach (var line in input[2..])
        {
            var match = Regex.Match(line, @"(.+) = \((.+), (.+)\)");
            _stepsMap[match.Groups[1].Value] = [match.Groups[2].Value, match.Groups[3].Value];
        }
    }

    public object SolvePart1() => TraverseFrom(Start, End);

    public object SolvePart2()
    {
        var allStarts = _stepsMap.Keys.Where(k => k[^1] == 'A').ToList();
        return TraverseFromMultiple(allStarts);
    }

    private int TraverseFrom(string current, string end)
    {
        var steps = 0;
        while (current != end)
        {
            var direction = _directions[steps++ % _directions.Length] == 'L' ? 0 : 1;
            current = _stepsMap[current][direction];
        }

        return steps;
    }

    private long TraverseFromMultiple(List<string> starts)
    {
        var steps = 0;
        var periodicity = new long[starts.Count]; // steps between each cyclical End-occurrence

        for (var i = 0; i < starts.Count; i++)
        {
            var current = starts[i];
            var firstOccurrence = 0;
            while (periodicity[i] == 0)
            {
                var direction = _directions[steps++ % _directions.Length] == 'L' ? 0 : 1;
                current = _stepsMap[current][direction];
                if (current[^1] != 'Z') continue; // not at an End node

                if (firstOccurrence == 0) firstOccurrence = steps;
                else periodicity[i] = steps - firstOccurrence;
            }
        }

        return periodicity.Aggregate(1L, Utils.LeastCommonMultiple);
    }
}