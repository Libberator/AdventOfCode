using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D04;

public class Solution : ISolver
{
    private readonly List<Pair> _pairs = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var d = line.Split('-', ',').ParseInts();
            _pairs.Add(new Pair(new Vec2D(d[0], d[1]), new Vec2D(d[2], d[3])));
        }
    }

    public object SolvePart1() => _pairs.Count(FullyContains);

    public object SolvePart2() => _pairs.Count(Overlaps);

    private static bool FullyContains(Pair p) =>
        (p.A.X <= p.B.X && p.A.Y >= p.B.Y) || (p.B.X <= p.A.X && p.B.Y >= p.A.Y);

    private static bool Overlaps(Pair p) => p.A.X <= p.B.Y && p.A.Y >= p.B.X;

    private record struct Pair(Vec2D A, Vec2D B);
}