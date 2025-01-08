using System.Collections.Generic;
using System.Collections.Immutable;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D16;

public class Solution : ISolver
{
    private string[] _grid = [];
    private Vec2D _startPos, _endPos;

    public void Setup(string[] input)
    {
        _grid = input;
        _startPos = new Vec2D(_grid.Length - 2, 1);
        _endPos = new Vec2D(1, _grid[1].Length - 2);
    }

    public object SolvePart1() => FindBestPath(_startPos, _endPos);

    public object SolvePart2() => FindBestPath(_startPos, _endPos, true);

    private int FindBestPath(Vec2D start, Vec2D end, bool part2 = false)
    {
        var startState = new State(new Pose2D(start, Vec2D.E), 0, [start]);
        var queue = new PriorityQueue<State, int>([(startState, 0)]);
        var minScores = new Dictionary<Pose2D, int>();
        var bestPaths = new HashSet<Vec2D>();
        var bestScore = int.MaxValue;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current.Score > bestScore) continue;

            if (current.Pose.Pos == end)
            {
                if (!part2) return current.Score;

                bestScore = current.Score;
                bestPaths.UnionWith(current.Path);
                continue;
            }

            if (_grid.GetAt(current.Pose.Ahead) != '#') EnqueueIfBetter(current.Step());
            if (_grid.GetAt(current.Pose.Left) != '#') EnqueueIfBetter(current.GoLeft());
            if (_grid.GetAt(current.Pose.Right) != '#') EnqueueIfBetter(current.GoRight());
        }

        return part2 ? bestPaths.Count : -1;

        void EnqueueIfBetter(State next)
        {
            if (minScores.TryGetValue(next.Pose, out var score) && next.Score > score) return;
            minScores[next.Pose] = next.Score;
            queue.Enqueue(next, next.Score);
        }
    }

    private readonly record struct State(Pose2D Pose, int Score, ImmutableStack<Vec2D> Path)
    {
        public State Step() => new(Pose.Step(), Score + 1, Path.Push(Pose.Ahead));
        public State GoRight() => new(Pose.TurnRight().Step(), Score + 1001, Path.Push(Pose.Right));
        public State GoLeft() => new(Pose.TurnLeft().Step(), Score + 1001, Path.Push(Pose.Left));
    }
}