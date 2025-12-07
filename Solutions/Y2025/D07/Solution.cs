using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2025.D07;

public class Solution : ISolver
{
    private const char Start = 'S', Splitter = '^';
    private string[] _input = [];
    private Vec2D _startPos;

    public void Setup(string[] input)
    {
        _input = input;
        _startPos = input.FindPosOf(Start);
    }

    public object SolvePart1() => Solve(_input, _startPos).TotalSplits;

    public object SolvePart2() => Solve(_input, _startPos).TotalPaths;

    private static (int TotalSplits, long TotalPaths) Solve(string[] input, Vec2D start)
    {
        var splits = 0;
        HashSet<int> columns = [start.Y];
        var pathCounts = new long[input[0].Length];
        pathCounts[start.Y] = 1;

        for (var x = start.X + 1; x < input.Length; x++)
            foreach (var y in new List<int>(columns))
            {
                if (input[x][y] != Splitter) continue;
                pathCounts[y - 1] += pathCounts[y];
                pathCounts[y + 1] += pathCounts[y];
                pathCounts[y] = 0;

                columns.Add(y - 1);
                columns.Add(y + 1);
                columns.Remove(y);
                splits++;
            }

        return (splits, pathCounts.Sum());
    }
}