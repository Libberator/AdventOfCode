using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D22;

public class Solution : ISolver
{
    private readonly HashSet<int> _diffHashesSeen = [];
    private readonly Dictionary<int, long> _diffPatternSums = [];
    private long[] _data = [];

    public void Setup(string[] input) => _data = input.ParseLongs();

    public object SolvePart1() => _data.Sum(GetSecretNumber);

    public object SolvePart2() => _diffPatternSums.Values.Max();

    private long GetSecretNumber(long n)
    {
        var buffer = new int[4];
        var lastDigit = (int)n % 10; // "last" as in both previous and in the ones place

        for (var i = 0; i < 2000; i++)
        {
            n ^= n << 6;
            n &= 0xFFFFFF; // n %= 16777216;
            n ^= n >> 5;
            n &= 0xFFFFFF; // n %= 0x1000000;
            n ^= n << 11;
            n &= 0xFFFFFF; // i.e. keep last 24 bits

            var digit = (int)n % 10;
            var diff = digit - lastDigit;
            buffer[i % 4] = diff;
            lastDigit = digit;

            if (i < 3) continue;

            var diffHash = Hash(buffer, i);
            if (!_diffHashesSeen.Add(diffHash)) continue;
            _diffPatternSums.AddToExistingOrCreate(diffHash, digit);
        }

        _diffHashesSeen.Clear();
        return n;

        static int Hash(int[] buffer, int i) => HashCode.Combine(
            buffer[(i - 3) % 4], buffer[(i - 2) % 4],
            buffer[(i - 1) % 4], buffer[i % 4]);
    }
}