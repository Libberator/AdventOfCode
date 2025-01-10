using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D16;

public class Solution : ISolver
{
    private const char ForwardSlash = '/', Backslash = '\\', VertSplitter = '|', HorizSplitter = '-';
    private string[] _grid = [];
    private Vec2D _gridSize;

    public void Setup(string[] input)
    {
        _grid = input;
        _gridSize = input.GetGridSize();
    }

    public object SolvePart1() => GetEnergizedCount(new Pose2D(new Vec2D(0, -1), Vec2D.E));

    public object SolvePart2()
    {
        var highScore = 0;

        Parallel.For(0, _gridSize.X, i =>
        {
            var score = GetEnergizedCount(new Pose2D(new Vec2D(i, -1), Vec2D.E));
            score = Math.Max(score, GetEnergizedCount(new Pose2D(new Vec2D(-1, i), Vec2D.S)));
            score = Math.Max(score, GetEnergizedCount(new Pose2D(_gridSize with { X = i }, Vec2D.W)));
            score = Math.Max(score, GetEnergizedCount(new Pose2D(_gridSize with { Y = i }, Vec2D.N)));

            if (score > highScore) Interlocked.Exchange(ref highScore, score);
        });

        return highScore;
    }

    private int GetEnergizedCount(Pose2D pose)
    {
        Queue<Pose2D> queue = new();
        HashSet<Pose2D> snapshots = [pose];
        HashSet<Vec2D> energized = [];
        queue.Enqueue(pose);

        while (queue.Count > 0)
        {
            var beam = queue.Dequeue();
            beam = beam.Step();

            if (!beam.Pos.IsWithinBounds(_gridSize) || !snapshots.Add(beam))
                continue;

            energized.Add(beam.Pos);

            switch (_grid.GetAt(beam.Pos))
            {
                case ForwardSlash:
                    queue.Enqueue(beam.Dir == Vec2D.W || beam.Dir == Vec2D.E
                        ? beam.TurnLeft()
                        : beam.TurnRight()); break;
                case Backslash:
                    queue.Enqueue(beam.Dir == Vec2D.W || beam.Dir == Vec2D.E ? beam.TurnRight() : beam.TurnLeft());
                    break;
                case VertSplitter when beam.Dir.X == 0: // Moving Horizontally
                case HorizSplitter when beam.Dir.Y == 0: // Moving Vertically
                    queue.Enqueue(beam.TurnLeft());
                    queue.Enqueue(beam.TurnRight());
                    break;
                default: queue.Enqueue(beam); break;
            }
        }

        return energized.Count;
    }
}