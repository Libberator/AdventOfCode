using System;
using System.Collections.Generic;
using AoC.Utilities.Collections;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D18;

public class Solution : ISolver
{
    private readonly List<Vec2D> _bytes = [];
    [TestValue(6, 6)] private readonly Vec2D _end = new(70, 70);
    [TestValue(12)] private readonly int _qty = 1024;
    private readonly Vec2D _start = Vec2D.Zero;

    public void Setup(string[] input) => _bytes.AddRange(input.ParseVec2Ds());

    public object SolvePart1() => FindPath([.._bytes[.._qty]]);

    public object SolvePart2() => _bytes[0.BinarySearch(_bytes.Count - 1, PathBlocked)];

    private bool PathBlocked(int i) => FindPath([.._bytes[..(i + 1)]], true) < 0;

    private int FindPath(HashSet<Vec2D> bytes, bool depthFirst = false)
    {
        Deque<Node> deque = new([new Node(_start, 0)]);
        HashSet<Vec2D> visited = [];

        Func<Node> getNextNode = depthFirst ? deque.RemoveFromBack : deque.RemoveFromFront; // DFS vs BFS

        while (deque.Count > 0)
        {
            var node = getNextNode();
            if (!visited.Add(node.Pos)) continue;

            if (node.Pos == _end) return node.Steps;

            foreach (var dir in Vec2D.CardinalDirs)
            {
                var next = node.Pos + dir;
                if (!next.IsWithinBoundsInclusive(_end) || visited.Contains(next) || bytes.Contains(next))
                    continue;
                deque.AddToBack(new Node(next, node.Steps + 1));
            }
        }

        return -1;
    }

    private readonly record struct Node(Vec2D Pos, int Steps);
}