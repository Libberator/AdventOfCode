using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D14;

public class Solution : ISolver
{
    private readonly HashSet<Vec2D> _sands = [];
    private readonly Vec2D _source = new(500, 0);
    private readonly HashSet<Vec2D> _walls = [];
    private int _maxDepth;

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var points = line.ParseVec2Ds();
            _maxDepth = Math.Max(points.Max(p => p.Y), _maxDepth);

            for (var i = 0; i < points.Length - 1; i++)
                foreach (var pos in Vec2D.GeneratePointsInclusive(points[i], points[i + 1]))
                    _walls.Add(pos);
        }

        _maxDepth += 2;
    }

    public object SolvePart1()
    {
        var stack = new Stack<Vec2D>([_source]);
        while (TryGetNextPosition(stack, out var settledPos))
            _sands.Add(settledPos);
        return _sands.Count;
    }

    public object SolvePart2()
    {
        var stack = new Stack<Vec2D>([_source]);
        var settledPos = Vec2D.Zero;
        while (settledPos != _source)
        {
            TryGetNextPosition(stack, out settledPos);
            _sands.Add(settledPos);
        }

        return _sands.Count;
    }

    private bool TryGetNextPosition(Stack<Vec2D> startingPoints, out Vec2D settlePos)
    {
        var pos = startingPoints.Pop();

        while (true)
        {
            settlePos = pos; // capture last known safe position to settle
            pos += new Vec2D(0, 1); // move down 1 step
            if (pos.Y >= _maxDepth) return false; // endless void (a.k.a. hit the floor)
            startingPoints.Push(settlePos); // cache a safe starting point
            if (!IsBlocked(pos)) continue; // can move down
            pos += new Vec2D(-1, 0); // can't move down, try down-left position
            if (!IsBlocked(pos)) continue; // can move left
            pos += new Vec2D(2, 0); // can't move left, try down-right position
            if (!IsBlocked(pos)) continue; // can move right
            startingPoints.Pop(); // remove last cached starting point as it will get filled
            return true; // can't move down/left/right, the sand settles
        }

        bool IsBlocked(Vec2D p) => _sands.Contains(p) || _walls.Contains(p);
    }
}