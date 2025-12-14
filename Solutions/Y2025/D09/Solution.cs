using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2025.D09;

public class Solution : ISolver
{
    private Vec2DLong[] _input = [];

    public void Setup(string[] input) => _input = input.ParseVec2DLongs();

    public object SolvePart1() => _input.UniquePairs().Max(pair => Area(pair.Item1, pair.Item2));

    public object SolvePart2()
    {
        long maxArea = 0;
        List<MinMaxRect> lines = [];

        for (int i = 0, j = _input.Length - 1; i < _input.Length; j = i++)
            lines.Add(new MinMaxRect(_input[i], _input[j]));

        foreach (var (a, b) in _input.UniquePairs())
        {
            var area = Area(a, b);
            if (area <= maxArea) continue;
            if (PassesAABB(new MinMaxRect(a, b), lines))
                maxArea = area;
        }

        return maxArea;
    }

    private static long Area(Vec2DLong a, Vec2DLong b) => (Math.Abs(a.Y - b.Y) + 1) * (Math.Abs(a.X - b.X) + 1);

    private static bool PassesAABB(MinMaxRect box, List<MinMaxRect> lines) => lines.All(l =>
        l.MaxY < box.MinY + 1 || l.MinY > box.MaxY - 1 || l.MaxX < box.MinX + 1 || l.MinX > box.MaxX - 1);

    private readonly struct MinMaxRect(Vec2DLong a, Vec2DLong b)
    {
        public long MinX { get; } = Math.Min(a.X, b.X);
        public long MaxX { get; } = Math.Max(a.X, b.X);
        public long MinY { get; } = Math.Min(a.Y, b.Y);
        public long MaxY { get; } = Math.Max(a.Y, b.Y);
    }
}