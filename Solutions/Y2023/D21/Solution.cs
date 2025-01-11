using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D21;

public class Solution : ISolver
{
    private const char Rock = '#'; //, Start = 'S';
    [TestValue(6)] private readonly int _part1Steps = 64;
    [TestValue(5000)] private readonly int _part2Steps = 26501365;
    private string[] _grid = [];
    private Vec2D _startPos;

    public void Setup(string[] input)
    {
        _grid = input;
        _startPos = input.GetGridSize() / 2; // in the center
    }

    public object SolvePart1() => TakeSteps(_grid, _startPos, _part1Steps).First();

    public object SolvePart2()
    {
        var size = _grid.Length;
        var rem = _part2Steps % size;

        // get 3 mapped points (y1, y2, y3) given 3 seed values (x1, x2, x3)
        var seq = TakeSteps(_grid, _startPos, rem, rem + size, rem + 2 * size).ToArray();

        // we can use the quadratic formula to extrapolate another y value given x
        // solve for a, b, and c in the form of y = a*x^2 + b*x + c given 3 sample points
        // Put simply, assume that sample point x1 is equal to 0, x2 is 1, and x3 is 2
        var a = (seq[2] - 2 * seq[1] + seq[0]) / 2;
        var b = seq[1] - seq[0] - a;
        var c = seq[0];

        var x = _part2Steps / size;
        var total = a * x * x + b * x + c;
        return total;
    }

    private static IEnumerable<long> TakeSteps(string[] grid, Vec2D startPos, params int[] steps)
    {
        long totalEvenPlots = 1, totalOddPlots = 0;
        HashSet<Vec2D> evenSteps = [startPos], oddSteps = [];

        if (steps.Contains(0))
            yield return totalEvenPlots;
        var maxSteps = steps.MaxBy(x => x);
        for (var i = 1; i <= maxSteps; i++)
        {
            if (i % 2 == 0)
                totalEvenPlots += Step(grid, oddSteps, evenSteps);
            else
                totalOddPlots += Step(grid, evenSteps, oddSteps);

            if (steps.Contains(i))
                yield return i % 2 == 0 ? totalEvenPlots : totalOddPlots;
        }
    }

    private static int Step(string[] grid, HashSet<Vec2D> fromSet, HashSet<Vec2D> toSet)
    {
        var toRemove = toSet.ToArray(); // snapshot copy

        foreach (var pos in fromSet)
            foreach (var dir in Vec2D.CardinalDirs)
            {
                var nextPos = pos + dir;
                if (CanVisit(grid, nextPos))
                    toSet.Add(nextPos);
            }

        toSet.ExceptWith(toRemove);
        return toSet.Count;
    }

    private static bool CanVisit(string[] grid, Vec2D nextPos)
    {
        var xPos = nextPos.X.Mod(grid.Length);
        var yPos = nextPos.Y.Mod(grid[0].Length);
        return grid[xPos][yPos] != Rock;
    }
}