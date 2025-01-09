using System;
using System.Collections.Generic;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2023.D13;

public class Solution : ISolver
{
    private readonly List<string[]> _patterns = [];

    public void Setup(string[] input)
    {
        var startIndex = 0;
        for (var i = 0; i < input.Length; i++)
        {
            if (!string.IsNullOrEmpty(input[i])) continue;
            _patterns.Add(input[startIndex..i]);
            startIndex = i + 1;
        }

        if (startIndex < input.Length && !string.IsNullOrEmpty(input[^1]))
            _patterns.Add(input[startIndex..]); // in case data ends w/o an empty line
    }

    public object SolvePart1() => GetTotalScore(_patterns, false);

    public object SolvePart2() => GetTotalScore(_patterns, true);

    private static int GetTotalScore(List<string[]> patterns, bool withSmudge)
    {
        var total = 0;
        foreach (var pattern in patterns)
            if (TryFindHorizontalReflection(pattern, withSmudge, out var row))
                total += 100 * row;
            else if (TryFindVerticalReflection(pattern, withSmudge, out var col))
                total += col;
        return total;
    }

    private static bool TryFindVerticalReflection(string[] pattern, bool withSmudge, out int foundCol) =>
        TryFindHorizontalReflection(pattern.Transpose(), withSmudge, out foundCol);

    private static bool TryFindHorizontalReflection(string[] pattern, bool withSmudge, out int foundRow)
    {
        foundRow = 0;
        for (var center = 0; center < pattern.Length - 1; center++)
        {
            if (!TryCheckReflection(pattern, center, withSmudge)) continue;
            foundRow = center + 1;
            return true;
        }

        return false;
    }

    private static bool TryCheckReflection(string[] pattern, int center, bool withSmudge)
    {
        var differences = 0;
        for (var i = 0; i <= center; i++)
        {
            var left = center - i;
            var right = center + i + 1;

            if (left < 0 || right >= pattern.Length) break;

            if (string.Equals(pattern[left], pattern[right], StringComparison.Ordinal)) continue;

            if (!withSmudge) return false;

            differences += CountDifferences(pattern[left], pattern[right]);
            if (differences > 1) return false;
        }

        return !withSmudge || differences == 1;
    }

    private static int CountDifferences(string left, string right)
    {
        var differences = 0;
        for (var i = 0; i < left.Length; i++)
            if (left[i] != right[i])
                differences++;
        return differences;
    }
}