using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D04;

public class Solution : ISolver
{
    private string[] _grid = [];
    private Vec2D _gridSize;

    public void Setup(string[] input)
    {
        _grid = input;
        _gridSize = _grid.GetGridSize();
    }

    public object SolvePart1() => _gridSize.GeneratePoints()
        .Where(pos => _grid.GetAt(pos) == 'X')
        .Sum(CountMasStrings);

    public object SolvePart2() => _gridSize.GeneratePoints(1)
        .Where(pos => _grid.GetAt(pos) == 'A')
        .Count(HasValidCorners);

    private int CountMasStrings(Vec2D pos) => Vec2D.AllDirs.Count(dir => IsStringFoundInDir(pos, dir));

    private bool IsStringFoundInDir(Vec2D pos, Vec2D dir, string str = "MAS")
    {
        foreach (var c in str)
        {
            pos += dir;
            if (!pos.IsWithinBounds(_gridSize) || _grid.GetAt(pos) != c) return false;
        }

        return true;
    }

    private bool HasValidCorners(Vec2D pos)
    {
        var chars = Vec2D.OrdinalDirs.Select(dir => _grid.GetAt(pos + dir)).ToArray();
        return chars.All(c => c is 'M' or 'S') && chars[0] != chars[2] && chars[1] != chars[3];
    }
}