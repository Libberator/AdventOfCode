using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D02;

public class Solution : ISolver
{
    private readonly List<Report> _reports = [];

    public void Setup(string[] input) => _reports.AddRange(input.Select(l => new Report(l.ParseInts())));

    public object SolvePart1() => _reports.Count(r => r.IsSafe());

    public object SolvePart2() => _reports.Count(r => r.IsWithinTolerance());

    private class Report(int[] source)
    {
        private bool? _isSafe;

        public bool IsSafe() => _isSafe ??= CheckIsSafe(source);
        public bool IsWithinTolerance() => IsSafe() || CheckSubsets(source);

        private static bool CheckIsSafe(int[] numbers)
        {
            if (numbers[1] - numbers[0] is 0 or > 3 or < -3) return false;
            var isIncreasing = numbers[1] > numbers[0];

            for (var i = 2; i < numbers.Length; i++)
            {
                if (isIncreasing != numbers[i] > numbers[i - 1]) return false;
                if (numbers[i] - numbers[i - 1] is 0 or > 3 or < -3) return false;
            }

            return true;
        }

        // brute forced. potential improvement: count # errors for direction and diff. If more than 1, then it fails 
        private static bool CheckSubsets(int[] numbers)
        {
            var subset = new int[numbers.Length - 1];
            for (var i = 0; i < numbers.Length; i++)
            {
                Array.Copy(numbers, 0, subset, 0, i);
                Array.Copy(numbers, i + 1, subset, i, numbers.Length - i - 1);
                if (CheckIsSafe(subset)) return true;
            }

            return false;
        }
    }
}