using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2025.D04;

public class Solution : ISolver
{
    private readonly HashSet<Vec2D> _papers = [];

    public void Setup(string[] input)
    {
        for (var x = 0; x < input.Length; x++)
        {
            var line = input[x];
            for (var y = 0; y < line.Length; y++)
                if (line[y] == '@')
                    _papers.Add(new Vec2D(x, y));
        }
    }

    public object SolvePart1() => _papers.Count(pos => Vec2D.AllDirs.Count(dir => _papers.Contains(pos + dir)) < 4);

    public object SolvePart2()
    {
        var startCount = _papers.Count;
        while (_papers.RemoveWhere(pos => Vec2D.AllDirs.Count(dir => _papers.Contains(pos + dir)) < 4) > 0)
        {
        }

        return startCount - _papers.Count;
    }
}