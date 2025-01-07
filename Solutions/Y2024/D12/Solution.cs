using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D12;

public class Solution : ISolver
{
    private readonly List<HashSet<Vec2D>> _plots = [];

    public void Setup(string[] input)
    {
        var gridSize = input.GetGridSize();

        foreach (var pos in gridSize.GeneratePoints())
        {
            if (_plots.Any(p => p.Contains(pos))) continue;

            HashSet<Vec2D> plot = [pos];
            var id = input.GetAt(pos);
            FloodFill(plot, pos, id, gridSize, input);
            _plots.Add(plot);
        }
    }

    public object SolvePart1() => _plots.Sum(p => Area(p) * Perimeter(p));

    public object SolvePart2() => _plots.Sum(p => Area(p) * NumberOfSides(p));

    private static void FloodFill(HashSet<Vec2D> plot, Vec2D pos, char id, Vec2D gridSize, string[] grid)
    {
        foreach (var next in Vec2D.CardinalDirs.Select(dir => pos + dir))
            if (next.IsWithinBounds(gridSize) && grid.GetAt(next) == id && plot.Add(next))
                FloodFill(plot, next, id, gridSize, grid);
    }

    private static int Area(HashSet<Vec2D> plot) => plot.Count;

    private static int Perimeter(HashSet<Vec2D> plot) =>
        plot.Sum(pos => Vec2D.CardinalDirs.Count(dir => !plot.Contains(pos + dir)));

    private static int NumberOfSides(HashSet<Vec2D> plot)
    {
        if (Area(plot) is 1 or 2) return 4;

        // find all the empty neighbors to account for island gaps
        var perimeters = plot.SelectMany(pos => Vec2D.CardinalDirs
            .Select(dir => pos + dir).Where(p => !plot.Contains(p))).ToHashSet();

        var sides = 0;
        while (perimeters.Count > 0)
            sides += WalkPerimeter(plot, perimeters);

        return sides;
    }

    private static int WalkPerimeter(HashSet<Vec2D> plot, HashSet<Vec2D> perimeter)
    {
        // 1. Find a point on the perimeter
        var startingPos = perimeter.First();
        var startingDir = Vec2D.CardinalDirs.First(dir => plot.Contains(startingPos - dir));
        startingDir = startingDir.RotatedRight();

        // 2. Count all the times we turn. # of corners == # of sides
        var turns = 0;
        var pos = startingPos;
        var dir = startingDir;

        do
        {
            perimeter.Remove(pos);

            // 3. Check if we should turn right
            if (!plot.Contains(pos + Vec2D.RotateRight(dir)))
            {
                dir = dir.RotatedRight();
                turns++;
                pos += dir;
                continue;
            }

            // 4. Or if we can just keep going straight
            if (!plot.Contains(pos + dir))
            {
                pos += dir;
                continue;
            }

            // 5. Otherwise, turn left
            dir = dir.RotatedLeft();
            turns++;
        } while (pos != startingPos || dir != startingDir);
        // 6. Repeat until we return back to the start

        return turns;
    }
}