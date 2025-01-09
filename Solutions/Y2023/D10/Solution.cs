using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Collections;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D10;

public class Solution : ISolver
{
    private const char Start = 'S', Vert = '|', Horiz = '-', Ll = 'L', Lr = 'J', Ur = '7', Ul = 'F';

    private static readonly Dictionary<Vec2D, char[]> DirsToTiles = new()
    {
        [Vec2D.N] = [Vert, Ul, Ur],
        [Vec2D.E] = [Horiz, Lr, Ur],
        [Vec2D.S] = [Vert, Ll, Lr],
        [Vec2D.W] = [Horiz, Ll, Ul]
    };

    private readonly HashSet<Vec2D> _path = [];
    private bool _isClockwise;

    public void Setup(string[] input)
    {
        var startingPos = input.FindPosOf(Start);
        InitializePath(_path, input, startingPos, out _isClockwise);
    }

    public object SolvePart1() => _path.Count / 2;

    public object SolvePart2() => GetEnclosedPositions(_path, _isClockwise).Count;

    private static void InitializePath(HashSet<Vec2D> path, string[] grid, Vec2D startingPos, out bool isClockwise)
    {
        var gridSize = grid.GetGridSize();
        var dir = GetStartingDir(grid, startingPos, gridSize);

        var current = startingPos + dir;
        var totalRightTurns = 0;

        path.Add(startingPos);
        while (current != startingPos)
        {
            path.Add(current);
            (dir, var turn) = NextDir(grid.GetAt(current), dir);
            current += dir;
            totalRightTurns += turn;
        }

        isClockwise = totalRightTurns > 0;
    }

    private static Vec2D GetStartingDir(string[] grid, Vec2D startingPos, Vec2D gridSize) =>
        Vec2D.CardinalDirs.First(dir =>
        {
            var pos = startingPos + dir;
            return pos.IsWithinBounds(gridSize) && DirsToTiles[dir].Contains(grid.GetAt(pos));
        });

    // -1 for left turn, +1 for right turn, and 0 for no turn
    private static (Vec2D Dir, int Turn) NextDir(char tile, Vec2D dir) => tile switch
    {
        Ur => dir == Vec2D.N ? (Vec2D.W, -1) : (Vec2D.S, 1),
        Lr => dir == Vec2D.E ? (Vec2D.N, -1) : (Vec2D.W, 1),
        Ll => dir == Vec2D.S ? (Vec2D.E, -1) : (Vec2D.N, 1),
        Ul => dir == Vec2D.W ? (Vec2D.S, -1) : (Vec2D.E, 1),
        _ => (dir, 0) // Vert or Horiz
    };

    private static HashSet<Vec2D> GetEnclosedPositions(HashSet<Vec2D> path, bool isClockwise)
    {
        HashSet<Vec2D> enclosedPositions = [];
        var pathBuffer = new CircularBuffer<Vec2D>(path); // so we can access by index more easily
        Func<Vec2D, Vec2D> rotateInside = isClockwise ? Vec2D.RotateRight : Vec2D.RotateLeft;

        for (var i = 0; i < pathBuffer.Capacity; i++)
        {
            var inFront = pathBuffer[i + 1];
            var current = pathBuffer[i];
            var behind = pathBuffer[i - 1];

            var insideForward = current + rotateInside(inFront - current);
            var insideBehind = current + rotateInside(current - behind);

            if (!path.Contains(insideForward)) enclosedPositions.Add(insideForward);
            if (!path.Contains(insideBehind)) enclosedPositions.Add(insideBehind);
        }

        // There are some deeper enclosed positions - do a flood-fill to get the rest
        foreach (var seed in enclosedPositions.ToArray())
            FloodFill(seed, path, enclosedPositions);

        return enclosedPositions;
    }

    private static void FloodFill(Vec2D seed, HashSet<Vec2D> path, HashSet<Vec2D> enclosedPositions)
    {
        foreach (var neighbor in Vec2D.CardinalDirs)
        {
            var pos = seed + neighbor;
            if (path.Contains(pos) || !enclosedPositions.Add(pos)) continue;
            FloodFill(pos, path, enclosedPositions);
        }
    }
}