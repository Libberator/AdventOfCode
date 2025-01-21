using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC.Solutions.Y2021.D08;

public class Solution : ISolver
{
    private readonly List<(string[] Patterns, string[] Outputs)> _data = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split('|', StringSplitOptions.TrimEntries);
            _data.Add((split[0].Split(), split[1].Split()));
        }
    }

    public object SolvePart1()
    {
        var sum = 0;
        foreach (var (_, outputs) in _data)
            sum += outputs.Count(output => output.Length is 2 or 3 or 4 or 7); // lengths for 1, 7, 4, and 8

        return sum;
    }

    public object SolvePart2()
    {
        var sum = 0;
        foreach (var (patterns, outputs) in _data)
            sum += GetFourDigitNumber(outputs, GenerateMapping(patterns));

        return sum;
    }

    // the index in the string[] correlates with the seven segment display digit
    private static string[] GenerateMapping(string[] patterns)
    {
        var mapping = new string[10];
        Array.Sort(patterns, (a, b) => a.Length.CompareTo(b.Length));

        // unique signal lengths: 1, 7, 4, 8 (same as Part 1)
        mapping[1] = patterns[0]; // unique 2 length
        mapping[7] = patterns[1]; // unique 3 length
        mapping[4] = patterns[2]; // unique 4 length
        mapping[8] = patterns[9]; // unique 7 length

        // signals of length five: 2, 3, 5
        for (var i = 3; i < 6; i++)
            // [3] - 7 = two lit. Notice that [5] - 7 has three lit, so it's safe to check for [3] first
            if (Diff(patterns[i], mapping[7]) == 2)
                mapping[3] = patterns[i];
            // [5] - 4 = two lit. Need to check this *second* or it will falsely flag for [3] - 4, which is also two lit 
            else if (Diff(patterns[i], mapping[4]) == 2)
                mapping[5] = patterns[i];
            // otherwise, it's a [2] because [2] minus a 7 or a 4 has three lit
            else
                mapping[2] = patterns[i];

        // signals of length six: 0, 6, 9
        for (var i = 6; i < 9; i++)
            // [9] - 3 = one lit
            if (Diff(patterns[i], mapping[3]) == 1)
                mapping[9] = patterns[i];
            // [6] - 5 = one lit
            else if (Diff(patterns[i], mapping[5]) == 1)
                mapping[6] = patterns[i];
            // otherwise it's a [0]
            else
                mapping[0] = patterns[i];

        return mapping;

        static int Diff(string orig, string remove) => orig.Count(c => !remove.Contains(c));
    }

    private static int GetFourDigitNumber(string[] outputs, string[] mapping)
    {
        var fourDigitNumber = 0;
        var multiplier = 1;

        for (var i = outputs.Length - 1; i >= 0; i--)
        {
            fourDigitNumber += multiplier * GetSingleDigit(outputs[i], mapping);
            multiplier *= 10;
        }

        return fourDigitNumber;
    }

    private static int GetSingleDigit(string output, string[] mapping)
    {
        for (var i = 0; i < mapping.Length; i++)
        {
            var pattern = mapping[i];
            if (pattern.Length != output.Length) continue;
            if (pattern.All(output.Contains)) return i;
        }

        throw new Exception("Something went wrong");
    }
}