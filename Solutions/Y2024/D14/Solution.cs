//#define PRINT_TREE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D14;

public class Solution : ISolver
{
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable ConvertToConstant.Local
    private const int Seconds = 100;
    [TestValue(11)] private static int _width = 101;
    [TestValue(7)] private static int _height = 103;

    private readonly List<Pose2D> _robots = [];

    public void Setup(string[] input)
    {
        _robots.AddRange(input.Select(line =>
        {
            var d = line.ParseInts();
            return new Pose2D(new Vec2D(d[0], d[1]), new Vec2D(d[2], d[3]));
        }));
    }

    public object SolvePart1()
    {
        var quadrants = new int[4];

        foreach (var (x, y) in _robots.Select(r => GetPosAtTime(r, Seconds)))
        {
            if (x == _width / 2 || y == _height / 2) continue;
            var i = 0;
            if (x > _width / 2) i += 1;
            if (y > _height / 2) i += 2;
            quadrants[i]++;
        }

        return quadrants.Product();
    }

    public object SolvePart2()
    {
        var x = Enumerable.Range(0, _width).MinBy(i => _robots.Select(r => GetPosAtTime(r, i).X).Variance());
        var y = Enumerable.Range(0, _height).MinBy(i => _robots.Select(r => GetPosAtTime(r, i).Y).Variance());
        var seconds = (_width.ModInverse(_height) * (y - x)).Mod(_height) * _width + x;

#if PRINT_TREE
        PrintState(seconds);
#endif
        return seconds;
    }

    private static Vec2D GetPosAtTime(Pose2D pose, int seconds)
    {
        var (x, y) = pose.Step(seconds).Pos;
        x = x.Mod(_width);
        y = y.Mod(_height);
        return new Vec2D(x, y);
    }

    // ReSharper disable once UnusedMember.Local
    private void PrintState(int seconds)
    {
        var positions = new HashSet<Vec2D>(_robots.Select(r => GetPosAtTime(r, seconds)));
        var sb = new StringBuilder();

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
                sb.Append(positions.Contains(new Vec2D(x, y)) ? '#' : '.');
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }
}