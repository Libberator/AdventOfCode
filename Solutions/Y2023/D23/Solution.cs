using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D23;

public class Solution : ISolver
{
    private const char Path = '.', Forest = '#', SlopeN = '^', SlopeE = '>', SlopeS = 'v', SlopeW = '<';
    private Dictionary<Vec2D, List<Edge>> _graph = new();
    private Vec2D _start, _end;

    public void Setup(string[] input)
    {
        _start = new Vec2D(0, 1);
        _end = new Vec2D(input.Length - 1, input[0].Length - 2);
        _graph = GenerateGraph(input, _start, _end);
    }

    public object SolvePart1() => FindLongestPath(_graph, _start, true);

    public object SolvePart2() => FindLongestPath(_graph, _start);

    private static Dictionary<Vec2D, List<Edge>> GenerateGraph(string[] grid, Vec2D start, Vec2D end)
    {
        Dictionary<Vec2D, List<Edge>> graph = [];
        List<Vec2D> nodes = [start, end];
        var gridSize = grid.GetGridSize();
        foreach (var pos in gridSize.GeneratePoints(1))
            if (grid.GetAt(pos) == Path && Vec2D.CardinalDirs.Count(dir => grid.GetAt(pos + dir) != Forest) >= 3)
                nodes.Add(pos);

        foreach (var node in nodes)
        {
            var edges = graph[node] = [];
            HashSet<Vec2D> visited = [node];
            Stack<(Vec2D, Vec2D, int)> stack = []; // pos, prevDir, steps
            stack.Push((node, Vec2D.Zero, 0));

            while (stack.Count > 0)
            {
                var (pos, prevDir, steps) = stack.Pop();

                if (nodes.Contains(pos) && steps != 0)
                {
                    var index = nodes.IndexOf(pos);
                    var symbol = grid.GetAt(pos - prevDir);
                    var isDirected = IsDirected(symbol, prevDir);
                    edges.Add(new Edge(pos, index, isDirected, steps));
                    continue;
                }

                foreach (var dir in Vec2D.CardinalDirs)
                {
                    var next = pos + dir;
                    if (!next.IsWithinBounds(gridSize) || grid.GetAt(next) == Forest || !visited.Add(next)) continue;
                    stack.Push((next, dir, steps + 1));
                }
            }
        }

        return graph;

        static bool IsDirected(char symbol, Vec2D prevDir) => prevDir == symbol switch
        {
            SlopeN => Vec2D.N, SlopeE => Vec2D.E, SlopeS => Vec2D.S, SlopeW => Vec2D.W, _ => prevDir
        };
    }

    // DFS
    private int FindLongestPath(Dictionary<Vec2D, List<Edge>> graph, Vec2D current, bool useDirected = false,
        long seenBitMask = 1L)
    {
        if (current == _end) return 0;

        var steps = int.MinValue;

        foreach (var edge in graph[current])
        {
            if (useDirected && !edge.IsDirected) continue;
            var nextBitMask = 1L << edge.NodeIndex;
            if ((seenBitMask & nextBitMask) > 0) continue;
            steps = Math.Max(steps,
                edge.Steps + FindLongestPath(graph, edge.To, useDirected, seenBitMask | nextBitMask));
        }

        return steps;
    }

    private record struct Edge(Vec2D To, int NodeIndex, bool IsDirected, int Steps);
}