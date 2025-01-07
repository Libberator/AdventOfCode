using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D10;

public class Solution : ISolver
{
    private readonly List<Vec2D> _trailheads = [];
    private Vec2D _bounds;
    private string[] _input = [];

    public void Setup(string[] input)
    {
        _input = input;
        _bounds = input.GetGridSize();
        foreach (var pos in _bounds.GeneratePoints())
            if (_input.GetAt(pos) == '0')
                _trailheads.Add(pos);
    }

    public object SolvePart1() => _trailheads.Sum(pos => FindTrails(pos).Distinct().Count());

    public object SolvePart2() => _trailheads.Sum(pos => FindTrails(pos).Count());

    private IEnumerable<Vec2D> FindTrails(Vec2D pos, char current = '0', char target = '9')
    {
        foreach (var dir in Vec2D.CardinalDirs)
        {
            var nextPos = pos + dir;
            if (!nextPos.IsWithinBounds(_bounds)) continue;
            var next = _input.GetAt(nextPos);
            if (next != current + 1) continue;

            if (next == target)
            {
                yield return nextPos;
                continue;
            }

            foreach (var trail in FindTrails(nextPos, next, target))
                yield return trail;
        }
    }
}