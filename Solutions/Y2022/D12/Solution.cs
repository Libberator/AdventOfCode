using System.Collections.Generic;
using AoC.Utilities.Algorithms;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D12;

public class Solution : ISolver
{
    private readonly Dictionary<Vec2D, HashSet<Vec2D>> _graph = [];
    private readonly Dictionary<Vec2D, HashSet<Vec2D>> _reversedGraph = [];
    private string[] _grid = [];
    private Vec2D _start, _end;

    public void Setup(string[] input)
    {
        _grid = input;
        _start = input.FindPosOf('S');
        _end = input.FindPosOf('E');
        _grid.SetAt(_start, 'a');
        _grid.SetAt(_end, 'z');

        var gridSize = input.GetGridSize();
        foreach (var pos in gridSize.GeneratePoints())
        {
            var value = _grid.GetAt(pos);
            var neighbors = _graph[pos] = [];
            foreach (var dir in Vec2D.CardinalDirs)
            {
                var neighbor = pos + dir;
                if (!neighbor.IsWithinBounds(gridSize) || _grid.GetAt(neighbor) - value > 1) continue;
                neighbors.Add(neighbor);
                _reversedGraph.TryAdd(neighbor, []);
                _reversedGraph[neighbor].Add(pos);
            }
        }
    }

    public object SolvePart1() => Pathfinding.FindShortestPath(_start, _end, _graph, GetCost, Heuristic).Count - 1;

    public object SolvePart2() => Pathfinding.FindShortestPath(_end, EndCondition, _reversedGraph, GetCost).Count - 1;

    private int GetCost(Vec2D _, Vec2D neighbor) => _grid.GetAt(neighbor) - 'a' + 1;
    private static int Heuristic(Vec2D pos, Vec2D end) => pos.DistanceManhattan(end);
    private bool EndCondition(Vec2D pos) => _grid.GetAt(pos) is 'a';// or 'S';
}