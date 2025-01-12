using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D08;

public class Solution : ISolver
{
    private string[] _grid = [];
    private Vec2D _gridSize;

    public void Setup(string[] input)
    {
        _grid = input;
        _gridSize = input.GetGridSize();
    }

    public object SolvePart1() => _gridSize.GeneratePoints().Count(IsVisible);

    public object SolvePart2() => _gridSize.GeneratePoints().Max(ScenicScore);

    private bool IsEdge(Vec2D pos) => pos.X == 0 || pos.Y == 0 || pos.X == _gridSize.X - 1 || pos.Y == _gridSize.Y - 1;

    private bool IsVisible(Vec2D pos)
    {
        var height = _grid.GetAt(pos);
        if (IsEdge(pos)) return true;
        foreach (var dir in Vec2D.CardinalDirs)
            if (WalkInDirection(pos, dir, height).CanSeeEdge)
                return true;
        return false;
    }

    private int ScenicScore(Vec2D pos)
    {
        var height = _grid.GetAt(pos);
        if (IsEdge(pos)) return 0;
        var score = 1;
        foreach (var dir in Vec2D.CardinalDirs)
            score *= WalkInDirection(pos, dir, height).ViewingDistance;
        return score;
    }

    private (bool CanSeeEdge, int ViewingDistance) WalkInDirection(Vec2D pos, Vec2D dir, char height)
    {
        var count = 0;
        while (!IsEdge(pos))
        {
            pos += dir;
            count++;
            if (_grid[pos.X][pos.Y] >= height) return (false, count);
        }

        return (true, count);
    }
}