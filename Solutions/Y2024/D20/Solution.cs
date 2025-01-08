using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D20;

public class Solution : ISolver
{
    private readonly Dictionary<Vec2D, int> _path = [];

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    // ReSharper disable once ConvertToConstant.Local
    [TestValue(50)] private int _minTimeSaved = 100;

    public void Setup(string[] input)
    {
        var start = input.FindPosOf('S');
        var end = input.FindPosOf('E');
        var dir = Vec2D.CardinalDirs.First(dir => input.GetAt(start + dir) != '#');

        FillPath(_path, input, start, dir, end);
    }

    public object SolvePart1() => TotalShortcuts(2);

    public object SolvePart2() => TotalShortcuts(20);

    private static void FillPath(Dictionary<Vec2D, int> path, string[] grid, Vec2D pos, Vec2D dir, Vec2D end)
    {
        var steps = 0;
        path.Add(pos, steps++);
        while (pos != end)
            foreach (var nextDir in Vec2D.CardinalDirs)
            {
                if (nextDir == -dir || grid.GetAt(pos + nextDir) == '#') continue;
                dir = nextDir;
                pos += dir;
                path.Add(pos, steps++);
                break;
            }
    }

    private int TotalShortcuts(int cheatDistance)
    {
        var shortcuts = 0;
        foreach (var (pos, steps) in _path)
            foreach (var pos2 in pos.GetDiamondPattern(cheatDistance, 1))
            {
                if (!_path.TryGetValue(pos2, out var steps2)) continue;
                if (steps2 - (steps + pos.DistanceManhattan(pos2)) >= _minTimeSaved) shortcuts++;
            }

        return shortcuts;
    }
}