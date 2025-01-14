using System;
using System.Collections.Generic;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D15;

public class Solution : ISolver
{
    private readonly List<Data> _data = [];
    [TestValue(10)] private readonly int _row = 2_000_000;

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var matches = Utils.NumberPattern().Matches(line);
            var sensor = new Vec2D(int.Parse(matches[0].ValueSpan), int.Parse(matches[1].ValueSpan));
            var beacon = new Vec2D(int.Parse(matches[2].ValueSpan), int.Parse(matches[3].ValueSpan));
            _data.Add(new Data(sensor, beacon, sensor.DistanceManhattan(beacon)));
        }
    }

    public object SolvePart1()
    {
        HashSet<int> beaconsAlongRow = [];
        SortedList<int, int> minMaxRanges = [];

        foreach (var data in _data)
        {
            if (data.Beacon.Y == _row)
                beaconsAlongRow.Add(data.Beacon.X);

            var deltaY = Math.Abs(_row - data.Sensor.Y);
            if (data.Distance < deltaY) continue; // doesn't touch the ROW

            var minX = data.Sensor.X - (data.Distance - deltaY);
            var maxX = data.Sensor.X + (data.Distance - deltaY);

            if (!minMaxRanges.TryAdd(minX, maxX))
                minMaxRanges[minX] = Math.Max(minMaxRanges[minX], maxX);
        }

        var occupiedCount = 0;
        var xMin = int.MinValue;
        foreach (var (min, max) in minMaxRanges)
        {
            xMin = Math.Max(xMin, min);
            if (xMin > max) continue;

            occupiedCount += max - xMin + 1;
            xMin = max + 1;
        }

        return occupiedCount - beaconsAlongRow.Count;
    }

    public object SolvePart2()
    {
        var beacon = FindBeacon();
        var tuningFrequency = beacon.X * 4_000_000L + beacon.Y;
        return tuningFrequency;
    }

    // This takes advantage of the fact that the hidden beacon must be just outside the edge of the sensor's range
    // Specifically, there will be 2 pairs of sensors to cover each side of the hidden beacon
    // Using the lines along the gaps between the pairs (they will be perpendicular), X marks the spot
    private Vec2D FindBeacon()
    {
        List<(Vec2D Point, Vec2D Dir)> lines = [];

        for (var i = 0; i < _data.Count - 1; i++)
        {
            var a = _data[i];
            for (var j = i + 1; j < _data.Count; j++)
            {
                var b = _data[j];
                var distance = a.Sensor.DistanceManhattan(b.Sensor);
                if (a.Distance + b.Distance + 2 != distance) continue;

                var slope = a.Sensor.X < b.Sensor.X == a.Sensor.Y < b.Sensor.Y
                    ? Vec2D.NE
                    : Vec2D.SE;
                if (lines.Count == 1 && lines[0].Dir == slope) // can't make an X with 2 parallel lines
                    continue;

                var point = a.Sensor.X < b.Sensor.X
                    ? a.Sensor + (a.Distance + 1) * Vec2D.S
                    : b.Sensor + (b.Distance + 1) * Vec2D.S;
                lines.Add((point, slope));
                if (lines.Count == 2) break;
            }

            if (lines.Count == 2) break;
        }

        var (pt1, dir1) = lines[0];
        var (pt2, dir2) = lines[1];
        Vec2D.LineIntersect(pt1, dir1, pt2, dir2, out var intersect);
        return intersect;
    }

    private record Data(Vec2D Sensor, Vec2D Beacon, int Distance);
}