using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2021.D05;

public class Solution : ISolver
{
    private readonly List<(Vec2D Start, Vec2D End)> _data = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split(" -> ");
            var start = Array.ConvertAll(split[0].Split(','), int.Parse);
            var end = Array.ConvertAll(split[1].Split(','), int.Parse);
            _data.Add((new Vec2D(start[0], start[1]), new Vec2D(end[0], end[1])));
        }
    }

    public object SolvePart1()
    {
        var spotsHit = new ConcurrentDictionary<Vec2D, int>();
        Parallel.ForEach(_data.Where(pair => pair.Start.IsLateralTo(pair.End)), pair =>
        {
            foreach (var point in pair.Start.GeneratePointsInclusive(pair.End))
                spotsHit.AddOrUpdate(point, 1, (_, v) => v + 1);
        });
        return spotsHit.Values.Count(val => val > 1);
    }

    public object SolvePart2()
    {
        var spotsHit = new ConcurrentDictionary<Vec2D, int>();
        Parallel.ForEach(_data, pair =>
        {
            var (start, end) = pair;
            var dir = (end - start).Normalized();

            spotsHit.AddOrUpdate(start, 1, (_, v) => v + 1);
            while (start != end)
            {
                start += dir;
                spotsHit.AddOrUpdate(start, 1, (_, v) => v + 1);
            }
        });
        return spotsHit.Values.Count(val => val > 1);
    }
}