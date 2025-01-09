using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D11;

public class Solution : ISolver
{
    private const char Galaxy = '#';
    private const long SpaceMultiplier = 1_000_000L;
    private readonly List<int> _emptyCols = [];
    private readonly List<int> _emptyRows = [];
    private readonly List<Vec2D> _galaxies = [];
    private long _emptySpaceCrossings;
    private long _unexpandedDistance;

    public void Setup(string[] input)
    {
        for (var i = 0; i < input.Length; i++)
        {
            // check cols for empty space. this assumes square dataset
            if (input.All(line => line[i] != Galaxy))
                _emptyCols.Add(i);

            // check rows for empty space
            var line = input[i];
            if (!line.Contains(Galaxy))
            {
                _emptyRows.Add(i);
                continue;
            }

            // locate galaxies
            for (var j = 0; j < line.Length; j++)
                if (line[j] == Galaxy)
                    _galaxies.Add(new Vec2D(i, j));
        }
    }

    public object SolvePart1()
    {
        for (var i = 0; i < _galaxies.Count - 1; i++)
        {
            var first = _galaxies[i];
            for (var j = i + 1; j < _galaxies.Count; j++)
            {
                var second = _galaxies[j];
                _unexpandedDistance += first.DistanceManhattan(second);
                _emptySpaceCrossings += EmptySpacesBetween(first, second);
            }
        }

        return _unexpandedDistance + _emptySpaceCrossings;
    }

    public object SolvePart2() => _unexpandedDistance + (SpaceMultiplier - 1) * _emptySpaceCrossings;

    private int EmptySpacesBetween(Vec2D a, Vec2D b)
    {
        var rowStart = Math.Min(a.X, b.X);
        var rowStop = Math.Max(a.X, b.X);
        var colStart = Math.Min(a.Y, b.Y);
        var colStop = Math.Max(a.Y, b.Y);

        return _emptyRows.Count(r => r > rowStart && r < rowStop) +
               _emptyCols.Count(c => c > colStart && c < colStop);
    }
}