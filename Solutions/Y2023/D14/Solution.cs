using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D14;

public class Solution : ISolver
{
    private const char Cube = '#', Round = 'O';
    private readonly HashSet<Vec2D> _cubes = [], _rounds = [];
    private int _size;

    public void Setup(string[] input)
    {
        _size = input.Length;
        foreach (var pos in input.GetGridSize().GeneratePoints())
            switch (input.GetAt(pos))
            {
                case Cube: _cubes.Add(pos); break;
                case Round: _rounds.Add(pos); break;
            }
    }

    public object SolvePart1()
    {
        TiltNorth();
        return GetScore(_rounds);
    }

    public object SolvePart2()
    {
        const int cycles = 1_000_000_000;
        Dictionary<int, int> cache = [];
        List<int> scores = [];

        for (var i = 0; i < cycles; i++)
        {
            DoCycle();
            scores.Add(GetScore(_rounds));
            var hash = GetHash(_rounds);
            if (cache.TryAdd(hash, i))
                continue;

            var prevIteration = cache[hash];
            var period = i - prevIteration;
            var finalIteration = prevIteration + (cycles - i) % period - 1;

            return scores[finalIteration];
        }

        return GetScore(_rounds);
    }

    private int GetScore(HashSet<Vec2D> rounds) => rounds.Sum(pos => _size - pos.X);

    private int GetHash(HashSet<Vec2D> rounds) =>
        rounds.Aggregate(0, (current, pos) => current ^ (pos.X * _size + pos.Y));

    private void DoCycle()
    {
        TiltNorth();
        TiltWest();
        TiltSouth();
        TiltEast();
    }

    private void TiltNorth() => Tilt(i => new Vec2D(0, i), i => new Vec2D(_size - 1, i), -Vec2D.N);
    private void TiltWest() => Tilt(i => new Vec2D(i, 0), i => new Vec2D(i, _size - 1), -Vec2D.W);
    private void TiltSouth() => Tilt(i => new Vec2D(_size - 1, i), i => new Vec2D(0, i), -Vec2D.S);
    private void TiltEast() => Tilt(i => new Vec2D(i, _size - 1), i => new Vec2D(i, 0), -Vec2D.E);

    private void Tilt(Func<int, Vec2D> startCreator, Func<int, Vec2D> endCreator, Vec2D dir)
    {
        for (var i = 0; i < _size; i++)
        {
            var end = endCreator(i);
            var start = startCreator(i);
            var emptyPos = GetNextEmptyPos(start, end, dir);
            var pos = emptyPos;

            while (pos != end)
            {
                pos += dir;

                if (_cubes.Contains(pos))
                {
                    emptyPos = GetNextEmptyPos(pos, end, dir);
                    pos = emptyPos;
                }
                else if (_rounds.Remove(pos))
                {
                    _rounds.Add(emptyPos);
                    emptyPos = GetNextEmptyPos(emptyPos + dir, end, dir);
                    pos = emptyPos;
                }
            }
        }
    }

    private Vec2D GetNextEmptyPos(Vec2D current, Vec2D end, Vec2D dir)
    {
        while (current != end)
        {
            if (!_cubes.Contains(current) && !_rounds.Contains(current))
                return current;
            current += dir;
        }

        return end;
    }
}