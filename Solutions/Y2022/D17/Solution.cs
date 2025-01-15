using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D17;

public class Solution : ISolver
{
    private const int ChamberWidth = 7;
    private const int ChamberFloor = 1;
    private const int SpawnYOffset = 4;
    private const int SpawnXOffset = 2;
    private static readonly Vec2D Gravity = new(0, -1);

    // each array contains the *offsets* for the pieces, using the bottom-left as the origin
    private static readonly Vec2D[][] Shapes =
    [
        [new Vec2D(0, 0), new Vec2D(1, 0), new Vec2D(2, 0), new Vec2D(3, 0)], // Horizontal
        [new Vec2D(1, 0), new Vec2D(0, 1), new Vec2D(1, 1), new Vec2D(2, 1), new Vec2D(1, 2)], // Plus
        [new Vec2D(0, 0), new Vec2D(1, 0), new Vec2D(2, 0), new Vec2D(2, 1), new Vec2D(2, 2)], // L
        [new Vec2D(0, 0), new Vec2D(0, 1), new Vec2D(0, 2), new Vec2D(0, 3)], // Vertical
        [new Vec2D(0, 0), new Vec2D(1, 0), new Vec2D(0, 1), new Vec2D(1, 1)] // Square
    ];

    private static string _input = "";

    public void Setup(string[] input) => _input = input[0];

    public object SolvePart1() => PlayPieces(2022);

    public object SolvePart2()
    {
        const long numberOfPieces = 1_000_000_000_000L;
        var seen = new Dictionary<string, (int Count, int Height)>();
        var tiles = new HashSet<Vec2D>();
        int height = 0, count = 0, inputIndex = 0;
        var hash = "";

        while (seen.TryAdd(hash, (count, height)))
        {
            height = PlayAPiece(count, tiles, height, ref inputIndex);
            hash = CreateHash(count++ % Shapes.Length, inputIndex, height, tiles);
        }

        var cycleLength = count - seen[hash].Count;
        var cycleHeight = height - seen[hash].Height;
        var numCycles = numberOfPieces / cycleLength;
        var remainder = numberOfPieces % cycleLength;

        return numCycles * cycleHeight + PlayPieces(remainder);
    }

    private static long PlayPieces(long count)
    {
        var tiles = new HashSet<Vec2D>();
        int height = 0, inputIndex = 0;

        for (var i = 0; i < count; i++)
            height = PlayAPiece(i, tiles, height, ref inputIndex);

        return height;
    }

    private static int PlayAPiece(int shapeIndex, HashSet<Vec2D> tiles, int height, ref int inputIndex)
    {
        var pos = GetSpawnPos(height);
        var shape = GetNextShape(shapeIndex);

        while (true)
        {
            var dir = GetNextDir(ref inputIndex);
            if (CanMove(pos, dir, shape, tiles))
                pos += dir;

            if (CanMove(pos, Gravity, shape, tiles))
            {
                pos += Gravity;
                continue;
            }

            foreach (var offset in shape)
                tiles.Add(pos + offset);

            var topOfPiece = pos.Y + shape.Max(s => s.Y);
            return Math.Max(height, topOfPiece);
        }
    }

    private static bool CanMove(Vec2D pos, Vec2D dir, Vec2D[] shape, HashSet<Vec2D> tiles)
    {
        foreach (var offset in shape)
        {
            var nextPos = pos + offset + dir;
            if (!nextPos.IsWithinBounds(0, ChamberWidth, ChamberFloor, int.MaxValue)) return false;
            if (tiles.Contains(nextPos)) return false;
        }

        return true;
    }

    private static Vec2D GetSpawnPos(int height) => new(SpawnXOffset, height + SpawnYOffset);

    private static Vec2D[] GetNextShape(int count) => Shapes[count % Shapes.Length];

    private static Vec2D GetNextDir(ref int index)
    {
        index %= _input.Length;
        return _input[index++] == '<' ? new Vec2D(-1, 0) : new Vec2D(1, 0);
    }

    private static string CreateHash(int shapeIndex, int inputIndex, int height, HashSet<Vec2D> tiles)
    {
        var sb = new StringBuilder();
        for (var x = 0; x < ChamberWidth; x++)
        {
            var depth = 0;
            while (!tiles.Contains(new Vec2D(x, height - depth)) && height - depth >= ChamberFloor)
                depth++;
            sb.Append($"{depth},");
        }

        sb.Append($"{shapeIndex},{inputIndex}");
        return sb.ToString();
    }
}