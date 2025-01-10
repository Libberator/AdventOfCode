using System;
using System.Collections.Generic;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D17;

public class Solution : ISolver
{
    private string[] _grid = [];

    public void Setup(string[] input) => _grid = input;

    public object SolvePart1() => GetMinHeatLoss(_grid, 3);

    public object SolvePart2() => GetMinHeatLoss(_grid, 10, 4);

    private static int GetMinHeatLoss(string[] grid, int maxConsecutive, int minConsecutive = 0)
    {
        PriorityQueue<Node, int> queue = new(1024);
        HashSet<int> seen = new(1024);
        queue.Enqueue(new Node(Vec2D.Zero, Vec2D.E, 0, 0), 0);
        queue.Enqueue(new Node(Vec2D.Zero, Vec2D.S, 0, 0), 0);
        var gridSize = grid.GetGridSize();
        var endPos = gridSize - Vec2D.One;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (!seen.Add(current.Snapshot())) continue;

            if (current.Consecutive < maxConsecutive)
            {
                var inFront = current.Pos + current.Dir;
                if (inFront.IsWithinBounds(gridSize))
                {
                    var heatLoss = current.HeatLoss + grid.GetAt(inFront).AsDigit();
                    var node = new Node(inFront, current.Dir, current.Consecutive + 1, heatLoss);
                    var fCost = heatLoss + inFront.DistanceManhattan(endPos);
                    queue.Enqueue(node, fCost);
                }
            }

            if (current.Consecutive < minConsecutive) continue;

            if (current.Pos == endPos) return current.HeatLoss;

            var left = current.Pos + current.Dir.RotatedLeft();
            if (left.IsWithinBounds(gridSize))
            {
                var heatLoss = current.HeatLoss + grid.GetAt(left).AsDigit();
                var node = new Node(left, current.Dir.RotatedLeft(), 1, heatLoss);
                var fCost = heatLoss + left.DistanceManhattan(endPos);
                queue.Enqueue(node, fCost);
            }

            var right = current.Pos + current.Dir.RotatedRight();
            if (right.IsWithinBounds(gridSize))
            {
                var heatLoss = current.HeatLoss + grid.GetAt(right).AsDigit();
                var node = new Node(right, current.Dir.RotatedRight(), 1, heatLoss);
                var fCost = heatLoss + right.DistanceManhattan(endPos);
                queue.Enqueue(node, fCost);
            }
        }

        return 0;
    }

    private readonly record struct Node(Vec2D Pos, Vec2D Dir, int Consecutive, int HeatLoss)
    {
        public int Snapshot() => HashCode.Combine(Pos, Dir, Consecutive);
    }
}