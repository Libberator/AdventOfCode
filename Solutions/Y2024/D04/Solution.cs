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
        .Sum(pos => Vec2D.AllDirs.Count(dir => IsStringFoundInDirection(pos, dir)));

    public object SolvePart2()
    {
        var total = 0;

        //Vec2D.GeneratePoints()
        
        // foreach (var pos in _gridSize.GeneratePoints())
        // {
        //     if (_grid.GetAt(pos) != 'A') continue;
        //     if (HasValidCorners(pos)) total++;
        // }
        
        
        // for (var row = 1; row < _input.Length - 1; row++)
        // {
        //     var line = _input[row];
        //     for (var col = 1; col < line.Length - 1; col++)
        //     {
        //         if (line[col] != 'A') continue;
        //         if (HasValidCorners(row, col))
        //             total++;
        //     }
        // }
        return total;
    }
    
    private bool IsStringFoundInDirection(Vec2D pos, Vec2D dir, string str = "MAS")
    {
        foreach (var c in str)
        {
            pos += dir;
            if (!pos.IsWithinBounds(_gridSize)) continue;
            if (_grid.GetAt(pos) != c) return false;
        }
        return true;
    }
    
    // private bool HasValidCorners(Vec2D pos)
    // {
    //     var ul = _input.GetAt() [row - 1][col - 1]; // upper-left
    //     if (ul is not 'M' and not 'S') return false;
    //     var ur = _input[row - 1][col + 1]; // upper-right
    //     if (ur is not 'M' and not 'S') return false;
    //     var bl = _input[row + 1][col - 1]; // bottom-left
    //     if (bl is not 'M' and not 'S') return false;
    //     var br = _input[row + 1][col + 1]; // bottom-right
    //     if (br is not 'M' and not 'S') return false;
    //     
    //     return ul != br && ur != bl;
    // }
}