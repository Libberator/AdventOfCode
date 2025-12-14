using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2025.D08;

public class Solution : ISolver
{
    [TestValue(10)] private readonly int _connectionCount = 1000;
    private readonly List<(long Cost, (Vec3DLong From, Vec3DLong To) Pair)> _distances = [];
    private Vec3DLong[] _junctionBoxes = [];

    public void Setup(string[] input)
    {
        _junctionBoxes = input.ParseVec3DLongs();

        for (var i = 0; i < _junctionBoxes.Length - 1; i++)
        {
            var a = _junctionBoxes[i];
            for (var j = i + 1; j < _junctionBoxes.Length; j++)
            {
                var b = _junctionBoxes[j];
                var cost = a.DistanceSquared(b);
                _distances.Add((cost, (a, b)));
            }
        }

        _distances.Sort((a, b) => a.Cost.CompareTo(b.Cost));
    }

    public object SolvePart1() => ConnectBoxes(_connectionCount);

    public object SolvePart2() => ConnectBoxes(int.MaxValue);

    private long ConnectBoxes(int qty)
    {
        var groups = _junctionBoxes.Select(j => new HashSet<Vec3DLong>([j])).ToList();
        foreach (var (_, (from, to)) in _distances.Take(qty))
        {
            var aIndex = groups.FindIndex(g => g.Contains(from));
            var bIndex = groups.FindIndex(g => g.Contains(to));
            if (aIndex == bIndex) continue;

            groups[bIndex].UnionWith(groups[aIndex]);
            groups.RemoveAt(aIndex);

            if (groups.Count == 1)
                return from.X * to.X;
        }

        return groups.Select(g => g.Count).Top(3).Product();
    }
}