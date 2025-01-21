using System;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2021.D09;

public class Solution : ISolver
{
    private string[] _grid = [];
    private int _rows, _cols;

    public void Setup(string[] input)
    {
        _grid = input;
        _rows = _grid.Length;
        _cols = _grid[0].Length;
    }

    public object SolvePart1()
    {
        var sum = 0;
        for (var x = 0; x < _rows; x++)
            for (var y = 0; y < _cols; y++)
                if (IsLowPoint(new Vec2D(x, y), out var value))
                    sum += 1 + value.AsDigit();

        return sum;
    }

    public object SolvePart2()
    {
        var topThree = new int[3];
        var visited = new bool[_rows, _cols];
        for (var x = 0; x < _rows; x++)
            for (var y = 0; y < _cols; y++)
            {
                if (visited[x, y] || _grid[x][y] == '9') continue;
                var basinSize = FloodFill(new Vec2D(x, y), visited);
                if (basinSize <= topThree[0]) continue;
                topThree[0] = basinSize;
                Array.Sort(topThree);
            }

        return topThree.Product();
    }

    private bool IsLowPoint(Vec2D pos, out char value)
    {
        value = _grid.GetAt(pos);
        foreach (var dir in Vec2D.CardinalDirs)
        {
            var neighbor = pos + dir;
            if (!neighbor.IsWithinBounds(_rows, _cols)) continue;
            if (_grid.GetAt(neighbor) <= value) return false;
        }

        return true;
    }

    private int FloodFill(Vec2D pos, bool[,] visited)
    {
        if (!pos.IsWithinBounds(_rows, _cols) || visited[pos.X, pos.Y] || _grid.GetAt(pos) == '9')
            return 0;

        visited[pos.X, pos.Y] = true;

        return FloodFill(pos + Vec2D.N, visited) +
               FloodFill(pos + Vec2D.E, visited) +
               FloodFill(pos + Vec2D.S, visited) +
               FloodFill(pos + Vec2D.W, visited) + 1;
    }
}