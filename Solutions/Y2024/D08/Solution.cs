using System.Collections.Generic;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D08;

public class Solution : ISolver
{
    private readonly Dictionary<char, List<Vec2D>> _antennas = [];
    private Vec2D _bounds;
    
    public void Setup(string[] input)
    {
        _bounds = input.GetGridSize();
        foreach (var pos in _bounds.GeneratePoints())
        {
            var c = input.GetAt(pos);
            if (c is '.' or '#') continue;
            if (!_antennas.TryAdd(c, [pos]))
                _antennas[c].Add(pos);
        }
    }

    public object SolvePart1()
    {
        HashSet<Vec2D> antiNodes = [];

        foreach (var (a, b) in GetEachPair(_antennas.Values))
        {
            var diff = b - a;
            if ((b + diff).IsWithinBounds(_bounds)) antiNodes.Add(b + diff);
            if ((a - diff).IsWithinBounds(_bounds)) antiNodes.Add(a - diff);
        }

        return antiNodes.Count;
    }

    public object SolvePart2()
    {
        HashSet<Vec2D> antiNodes = [];

        foreach (var (a, b) in GetEachPair(_antennas.Values))
        {
            var diff = b - a;
            var pos = a;
            while ((pos += diff).IsWithinBounds(_bounds)) antiNodes.Add(pos);
            pos = b;
            while ((pos -= diff).IsWithinBounds(_bounds)) antiNodes.Add(pos);
        }

        return antiNodes.Count;
    }
    
    private static IEnumerable<(Vec2D a, Vec2D b)> GetEachPair(ICollection<List<Vec2D>> antennas)
    {
        foreach (var nodes in antennas)
            for (var i = 0; i < nodes.Count - 1; i++)
                for (var j = i + 1; j < nodes.Count; j++)
                    yield return (nodes[i], nodes[j]);
    }
}