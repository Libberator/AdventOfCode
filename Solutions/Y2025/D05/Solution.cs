using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2025.D05;

public class Solution : ISolver
{
    private readonly List<Vec2DLong> _ranges = [];
    private readonly List<long> _values = [];

    public void Setup(string[] input)
    {
        var splitIndex = Array.FindIndex(input, string.IsNullOrEmpty);
        foreach (var line in input[..splitIndex])
            _ranges.Add(Vec2DLong.Parse(line.Replace('-', ',')));
        foreach (var line in input[(splitIndex + 1)..])
            _values.Add(long.Parse(line));
    }

    public object SolvePart1() => _values.Count(value => _ranges.Any(range => value >= range.X && value <= range.Y));

    public object SolvePart2()
    {
        for (var i = _ranges.Count - 1; i >= 1; i--)
        {
            var a = _ranges[i];
            for (var j = 0; j < i; j++)
            {
                var b = _ranges[j];
                if (!Overlaps(a, b)) continue;
                _ranges[j] = new Vec2DLong(Math.Min(a.X, b.X), Math.Max(a.Y, b.Y));
                _ranges.RemoveAt(i);
                break;
            }
        }

        return _ranges.Sum(range => range.Y - range.X + 1);

        static bool Overlaps(Vec2DLong a, Vec2DLong b) => a.X <= b.Y && b.X <= a.Y;
    }
}