using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D03;

public class Solution : ISolver
{
    private readonly Dictionary<(Vec2D, char), List<int>> _parts = [];

    public void Setup(string[] input)
    {
        var gridSize = input.GetGridSize();
        foreach (var pos in gridSize.GeneratePoints())
        {
            var c = input.GetAt(pos);
            if (char.IsDigit(c) || c == '.') continue;
            var numbers = new List<int>();
            PopulateWithNumbers(input, gridSize, pos, numbers);
            if (numbers.Count == 0) continue;
            _parts[(pos, c)] = numbers;
        }
    }

    public object SolvePart1() => _parts.Values.Sum(numbers => numbers.Sum());

    public object SolvePart2() => _parts
        .Where(kvp => kvp.Key.Item2 == '*' && kvp.Value.Count == 2)
        .Sum(kvp => kvp.Value.Product());

    private static void PopulateWithNumbers(string[] input, Vec2D gridSize, Vec2D symbolPos, List<int> numbers)
    {
        HashSet<Vec2D> visited = [];
        foreach (var dir in Vec2D.AllDirs)
        {
            var pos = symbolPos + dir;
            if (!pos.IsWithinBounds(gridSize) || !visited.Add(pos) || !char.IsDigit(input.GetAt(pos))) continue;

            var row = pos.X;
            int l = pos.Y, r = pos.Y;
            while (l > 0 && char.IsDigit(input[row][l - 1]))
            {
                l--;
                visited.Add(new Vec2D(row, l));
            }

            while (r < gridSize.X - 1 && char.IsDigit(input[row][r + 1]))
            {
                r++;
                visited.Add(new Vec2D(row, r));
            }

            var number = int.Parse(input[row][l..(r + 1)]);
            numbers.Add(number);
        }
    }
}