using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D23;

public class Solution : ISolver
{
    private const char Elf = '#';
    private static readonly Vec2D[] NorthRegion = [Vec2D.N, Vec2D.NE, Vec2D.NW];
    private static readonly Vec2D[] SouthRegion = [Vec2D.S, Vec2D.SE, Vec2D.SW];
    private static readonly Vec2D[] WestRegion = [Vec2D.W, Vec2D.NW, Vec2D.SW];
    private static readonly Vec2D[] EastRegion = [Vec2D.E, Vec2D.NE, Vec2D.SE];
    private static readonly Queue<Vec2D[]> Quadrants = new([NorthRegion, SouthRegion, WestRegion, EastRegion]);
    private readonly HashSet<Vec2D> _positions = [];

    public void Setup(string[] input)
    {
        foreach (var pos in input.GetGridSize().GeneratePoints())
            if (input.GetAt(pos) == Elf)
                _positions.Add(pos);
    }

    public object SolvePart1()
    {
        for (var i = 0; i < 10; i++)
            MoveElves(_positions);

        Vec2D lowerCorner = _positions.First(), upperCorner = _positions.First();
        foreach (var pos in _positions)
        {
            lowerCorner = Vec2D.Min(lowerCorner, pos);
            upperCorner = Vec2D.Max(upperCorner, pos);
        }

        var area = (upperCorner.X - lowerCorner.X + 1) * (upperCorner.Y - lowerCorner.Y + 1);
        var elves = _positions.Count;
        return area - elves;
    }

    public object SolvePart2()
    {
        var roundNumber = 11;
        while (MoveElves(_positions)) roundNumber++;
        return roundNumber;
    }

    private static bool MoveElves(HashSet<Vec2D> grid)
    {
        Dictionary<Vec2D, Vec2D> fromToProposals = [];
        Dictionary<Vec2D, int> targetCount = [];

        // First Half - Check the 8 direction, if too close to another elf, propose a spot to move to
        foreach (var pos in grid)
        {
            if (!Vec2D.AllDirs.Any(dir => grid.Contains(pos + dir))) continue;
            if (!TryGetNextPosition(pos, grid, out var moveToPos)) continue;
            fromToProposals[pos] = moveToPos;
            targetCount.AddToExistingOrCreate(moveToPos, 1);
        }

        if (targetCount.All(kvp => kvp.Value > 1)) 
            return false; // either empty or nothing can move

        // Second Half - Move each elf to their proposed position, only if there's just 1 going there
        foreach (var (from, to) in fromToProposals)
        {
            if (targetCount[to] > 1) continue;
            grid.Remove(from);
            grid.Add(to);
        }

        // Finally - Cycle the first region from the front to the back
        Quadrants.Enqueue(Quadrants.Dequeue());
        return true;
    }

    private static bool TryGetNextPosition(Vec2D pos, HashSet<Vec2D> grid, out Vec2D nextPos)
    {
        nextPos = Vec2D.Zero;
        foreach (var region in Quadrants)
        {
            nextPos = pos + region[0];
            if (region.All(dir => !grid.Contains(pos + dir))) return true;
        }

        return false;
    }
}