using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D15;

public class Solution : ISolver
{
    private readonly HashSet<Vec2D> _boxes = [];
    private readonly HashSet<Vec2D> _walls = [];
    private string _instructions = "";
    private Vec2D _startPos;

    public void Setup(string[] input)
    {
        var chunks = input.ChunkByNonEmpty();
        _instructions = string.Join("", chunks[1]);

        foreach (var pos in chunks[0].GetGridSize().GeneratePoints())
            switch (chunks[0].GetAt(pos))
            {
                case '#': _walls.Add(pos); break;
                case 'O': _boxes.Add(pos); break;
                case '@': _startPos = pos; break;
            }
    }

    public object SolvePart1()
    {
        var boxes = new HashSet<Vec2D>(_boxes);
        var pos = _startPos;

        foreach (var dir in _instructions.Select(ToVector))
        {
            if (!CanMove(dir, out var nextEmptyPos)) continue;
            pos += dir;
            if (boxes.Remove(pos)) boxes.Add(nextEmptyPos);
        }

        return boxes.Sum(b => 100 * b.X + b.Y);

        bool CanMove(Vec2D dir, out Vec2D nextEmptyPos)
        {
            nextEmptyPos = pos;
            while (true)
            {
                nextEmptyPos += dir;
                if (_walls.Contains(nextEmptyPos)) return false;
                if (boxes.Contains(nextEmptyPos)) continue;
                return true;
            }
        }
    }

    public object SolvePart2()
    {
        var boxes = new HashSet<Vec2D>(_boxes.Select(Widen));
        var walls = new HashSet<Vec2D>(_walls.Select(Widen));
        var boxesToMove = new HashSet<Vec2D>();
        var pos = Widen(_startPos);

        foreach (var dir in _instructions.Select(ToVector))
        {
            boxesToMove.Clear();
            if (!CanMove(pos, dir)) continue;
            pos += dir;
            foreach (var box in boxesToMove.Reverse())
                if (boxes.Remove(box))
                    boxes.Add(box + dir);
        }

        return boxes.Sum(b => 100 * b.X + b.Y);

        bool CanMove(Vec2D nextPos, Vec2D dir)
        {
            nextPos += dir;
            if (walls.Contains(nextPos) || walls.Contains(nextPos + Vec2D.W)) return false;

            if (boxes.Contains(nextPos))
            {
                boxesToMove.Add(nextPos);
                return (IsHorizontal(dir) || CanMove(nextPos, dir)) && CanMove(nextPos + Vec2D.E, dir);
            }

            if (!boxes.Contains(nextPos + Vec2D.W)) return true;

            boxesToMove.Add(nextPos + Vec2D.W);
            return (IsHorizontal(dir) || CanMove(nextPos, dir)) && CanMove(nextPos + Vec2D.W, dir);

            bool IsHorizontal(Vec2D direction) => direction == Vec2D.E || direction == Vec2D.W;
        }
    }

    private static Vec2D Widen(Vec2D v) => v with { Y = 2 * v.Y };

    private static Vec2D ToVector(char input) => input switch
    {
        '^' => Vec2D.N,
        '>' => Vec2D.E,
        'v' => Vec2D.S,
        '<' => Vec2D.W,
        _ => throw new Exception()
    };
}