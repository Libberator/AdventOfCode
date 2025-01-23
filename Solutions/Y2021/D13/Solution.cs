using System;
using System.Collections.Generic;
using System.Text;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2021.D13;

public class Solution : ISolver
{
    private const char XAxis = 'x', YAxis = 'y';
    private const int LetterHeight = 6, LetterWidth = 4;
    private readonly List<(char Axis, int Pos)> _directions = [];
    private readonly List<Vec2D> _points = [];

    public void Setup(string[] input)
    {
        var emptyLineIndex = Array.IndexOf(input, string.Empty);

        foreach (var line in input[..emptyLineIndex])
        {
            var split = line.Split(',');
            _points.Add(new Vec2D(int.Parse(split[0]), int.Parse(split[1])));
        }

        foreach (var line in input[(emptyLineIndex + 1)..])
        {
            var split = line.Split('=');
            _directions.Add((split[0][^1], int.Parse(split[1])));
        }
    }

    public object SolvePart1()
    {
        var (axis, pos) = _directions[0];
        FoldAt(axis, pos, _points);
        return new HashSet<Vec2D>([.._points]).Count;
    }

    public object SolvePart2()
    {
        foreach (var (axis, pos) in _directions)
            FoldAt(axis, pos, _points);

        return GetDisplay([.._points]);
    }

    private static void FoldAt(char axis, int pos, List<Vec2D> points)
    {
        for (var i = 0; i < points.Count; i++)
            points[i] = axis switch
            {
                XAxis => RotateAcrossVerticalLine(points[i], pos),
                YAxis => RotateAcrossHorizontalLine(points[i], pos),
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
    }

    private static Vec2D RotateAcrossVerticalLine(Vec2D point, int xPos) =>
        point.X > xPos ? point with { X = point.X - 2 * (point.X - xPos) } : point;

    private static Vec2D RotateAcrossHorizontalLine(Vec2D point, int yPos) =>
        point.Y > yPos ? point with { Y = point.Y - 2 * (point.Y - yPos) } : point;

    private static string GetDisplay(HashSet<Vec2D> points, int numberOfLetters = 8)
    {
        var width = numberOfLetters * LetterWidth + (numberOfLetters - 1);

        var sb = new StringBuilder();
        for (var row = 0; row < LetterHeight; row++)
        {
            sb.Append('\n');
            for (var col = 0; col < width; col++)
                sb.Append(points.Contains(new Vec2D(col, row)) ? "#" : ".");
        }

        return sb.ToString();
    }
}