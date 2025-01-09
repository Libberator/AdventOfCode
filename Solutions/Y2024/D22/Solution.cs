using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D22;

public class Solution : ISolver
{
    private long[] _data = [];

    public void Setup(string[] input) => _data = input.ParseLongs();

    public object SolvePart1() => _data.Sum(GetSecretNumber);

    public object SolvePart2() => GetDiffPatternSums(_data).Values.Max();

    private static long GetSecretNumber(long n)
    {
        for (var i = 0; i < 2000; i++)
            n = ProcessNumber(n);
        return n;
    }

    private static long ProcessNumber(long n)
    {
        n ^= n << 6; // Mix with `n * 64`
        n &= 0xFFFFFF; // Prune. `n %= 16777216` == `n %= 0x1000000` == keep last 24 bits
        n ^= n >> 5; // Mix with `n / 32`
        //n &= 0xFFFFFF; // Prune can be skipped here
        n ^= n << 11; // Mix with `n * 2048`
        n &= 0xFFFFFF; // Prune
        return n;
    }

    private static Dictionary<int, long> GetDiffPatternSums(long[] numbers)
    {
        Dictionary<int, long> diffPatternSums = [];
        foreach (var number in numbers)
        {
            var n = number;
            var lastDigit = (int)n % 10; // "last" as in both "previous" and in the "ones place"
            var diffHashesSeen = new HashSet<int>();
            var circularBuffer = new int[4];

            for (var i = 0; i < 2000; i++)
            {
                n = ProcessNumber(n);

                var digit = (int)n % 10;
                var diff = digit - lastDigit;
                circularBuffer[i % 4] = diff;
                lastDigit = digit;

                if (i < 3) continue;

                var diffHash = Hash(circularBuffer, i);
                if (!diffHashesSeen.Add(diffHash)) continue;
                diffPatternSums.AddToExistingOrCreate(diffHash, digit);
            }
        }

        return diffPatternSums;

        static int Hash(int[] buffer, int i) => HashCode.Combine(
            buffer[(i - 3) % 4], buffer[(i - 2) % 4],
            buffer[(i - 1) % 4], buffer[i % 4]);
    }
}