using System.Collections.Generic;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D24;

public class Solution : ISolver
{
    private static readonly Dictionary<int, List<int>> DownCol2RowIndices = new();
    private static readonly Dictionary<int, List<int>> LeftRow2ColIndices = new();
    private static readonly Dictionary<int, List<int>> RightRow2ColIndices = new();
    private static readonly Dictionary<int, List<int>> UpCol2RowIndices = new();
    private static Vec2D _gridSize;
    private Vec2D _start, _end;
    private int _time;

    public void Setup(string[] input)
    {
        _gridSize = input.GetGridSize() - 2 * Vec2D.One;
        _start = new Vec2D(-1, 0);
        _end = new Vec2D(_gridSize.X, _gridSize.Y - 1);

        for (var row = 1; row < input.Length - 1; row++)
        {
            for (var col = 1; col < input[0].Length - 1; col++)
                switch (input[row][col])
                {
                    // adjusting them all to be 0-indexed for easier modulus math later
                    case '^': AddToOrCreate(UpCol2RowIndices, col - 1, row - 1); break;
                    case 'v': AddToOrCreate(DownCol2RowIndices, col - 1, row - 1); break;
                    case '<': AddToOrCreate(LeftRow2ColIndices, row - 1, col - 1); break;
                    case '>': AddToOrCreate(RightRow2ColIndices, row - 1, col - 1); break;
                }
        }

        return;

        static void AddToOrCreate(Dictionary<int, List<int>> target, int key, int value)
        {
            target.TryAdd(key, []);
            target[key].Add(value);
        }
    }

    public object SolvePart1() => _time = FindQuickestPath(_start, _end);

    public object SolvePart2() => FindQuickestPath(_start, _end, FindQuickestPath(_end, _start, _time));

    // Uses A* Pathfinding Logic
    private static int FindQuickestPath(Vec2D start, Vec2D end, int startingMinutes = 0)
    {
        HashSet<State> processed = [];
        PriorityQueue<State, int> queue = new();
        queue.Enqueue(new State(start, startingMinutes), 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!processed.Add(current)) continue;

            var nextMinute = current.Minutes + 1;

            foreach (var dir in Vec2D.CardinalDirs)
            {
                var nextPos = current.Pos + dir;
                if (nextPos == end) return nextMinute;
                if (!nextPos.IsWithinBounds(_gridSize) || !IsValidMovePosition(nextPos, nextMinute)) continue;

                var nextState = new State(nextPos, nextMinute);
                if (processed.Contains(nextState)) continue;

                var cost = nextMinute + nextPos.DistanceManhattan(end);
                queue.Enqueue(nextState, cost);
            }

            if (!IsValidMovePosition(current.Pos, nextMinute)) continue;

            var stationaryState = current with { Minutes = nextMinute };
            if (processed.Contains(stationaryState)) continue;

            var stationaryCost = nextMinute + current.Pos.DistanceManhattan(end);
            queue.Enqueue(stationaryState, stationaryCost);
        }

        return 0;
    }

    private static bool IsValidMovePosition(Vec2D pos, int minute)
    {
        if (UpCol2RowIndices.TryGetValue(pos.Y, out var rowIndicesUp) &&
            rowIndicesUp.Contains((pos.X + minute).Mod(_gridSize.X))) return false;

        if (DownCol2RowIndices.TryGetValue(pos.Y, out var rowIndicesDown) &&
            rowIndicesDown.Contains((pos.X - minute).Mod(_gridSize.X))) return false;

        if (LeftRow2ColIndices.TryGetValue(pos.X, out var colIndicesLeft) &&
            colIndicesLeft.Contains((pos.Y + minute).Mod(_gridSize.Y))) return false;

        if (RightRow2ColIndices.TryGetValue(pos.X, out var colIndicesRight) &&
            colIndicesRight.Contains((pos.Y - minute).Mod(_gridSize.Y))) return false;

        return true;
    }

    private readonly record struct State(Vec2D Pos, int Minutes);
}