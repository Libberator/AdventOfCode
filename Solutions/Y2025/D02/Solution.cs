using System;
using System.Collections.Generic;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2025.D02;

public class Solution : ISolver
{
    private readonly List<Vec2DLong> _idRanges = [];

    public void Setup(string[] input)
    {
        foreach (var pair in input[0].Split(','))
            _idRanges.Add(Vec2DLong.Parse(pair.Replace('-', ',')));
    }

    public object SolvePart1()
    {
        long total = 0;
        foreach (var range in _idRanges)
        {
            var (start, end) = range;
            // adjust start and end range to be within an even number of digits
            var startDigits = (int)Math.Log10(start) + 1;
            if (startDigits % 2 != 0) start = (long)Math.Pow(10, startDigits);
            var endDigits = (int)Math.Log10(end) + 1;
            if (endDigits % 2 != 0) end = (long)Math.Pow(10, endDigits - 1) - 1;

            var halfPower = (long)Math.Pow(10, (startDigits + 1) >> 1);
            for (var i = start; i <= end; i++)
                if (i / halfPower == i % halfPower)
                    total += i;
        }

        return total;
    }

    public object SolvePart2()
    {
        long total = 0;
        foreach (var (start, end) in _idRanges)
            for (var i = start; i <= end; i++)
            {
                var digits = (int)Math.Log10(i) + 1;
                for (var n = 1; n <= digits / 2; n++)
                {
                    if (digits % n != 0) continue;
                    var power = (long)Math.Pow(10, n);
                    var segment = i % power;
                    var repeats = digits / n;
                    var segmentDuplicator = ((long)Math.Pow(power, repeats) - 1) / (power - 1);
                    var repeatedValue = segment * segmentDuplicator;
                    if (i != repeatedValue) continue;
                    total += i;
                    break;
                }
            }

        return total;
    }
}