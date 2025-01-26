using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Algorithms;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2021.D15;

public class Solution : ISolver
{
    private readonly Vec2D _start = Vec2D.Zero;
    private string[] _grid = [];
    private Vec2D _gridSize, _expandedGridSize;

    public void Setup(string[] input)
    {
        _grid = input;
        _gridSize = input.GetGridSize();
        _expandedGridSize = _gridSize * 5;
    }

    public object SolvePart1()
    {
        var goal = _gridSize - Vec2D.One;
        var path = Pathfinding.FindShortestPath(_start, goal, GetNeighbors, GetCost, Vec2D.DistanceManhattan);
        return path.Sum(CostAt) - CostAt(_start);
    }

    public object SolvePart2()
    {
        var goal = _expandedGridSize - Vec2D.One;
        var path = Pathfinding.FindShortestPath(_start, goal, GetNeighborsExpanded, GetCostExpanded,
            Vec2D.DistanceManhattan);
        return path.Sum(CostAtExpanded) - CostAt(_start);
    }

    private IEnumerable<Vec2D> GetNeighbors(Vec2D pos)
    {
        foreach (var dir in Vec2D.CardinalDirs)
        {
            var next = pos + dir;
            if (!next.IsWithinBounds(_gridSize)) continue;
            yield return next;
        }
    }

    private int CostAt(Vec2D pos) => _grid.GetAt(pos) - '0';

    private int GetCost(Vec2D _, Vec2D next) => CostAt(next);

    private IEnumerable<Vec2D> GetNeighborsExpanded(Vec2D pos)
    {
        foreach (var dir in Vec2D.CardinalDirs)
        {
            var next = pos + dir;
            if (!next.IsWithinBounds(_expandedGridSize)) continue;
            yield return next;
        }
    }

    private int CostAtExpanded(Vec2D pos)
    {
        var riskIncrease = pos.X / _gridSize.X + pos.Y / _gridSize.Y;
        var cost = CostAt(pos % _gridSize) + riskIncrease;
        return (cost - 1) % 9 + 1; // same as `return cost > 9 ? cost - 9 : cost;` but less branching
    }

    private int GetCostExpanded(Vec2D _, Vec2D next) => CostAtExpanded(next);
}