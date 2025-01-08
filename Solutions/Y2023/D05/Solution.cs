using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D05;

public class Solution : ISolver
{
    private readonly List<Map> _maps = [];
    private readonly List<Vec2DLong> _seedRanges = [];
    private long[] _seeds = [];

    public void Setup(string[] input)
    {
        _seeds = input[0]["seeds: ".Length..].ParseLongs();
        _seedRanges.AddRange(_seeds.Chunk(2).Select(x => new Vec2DLong(x[0], x[0] + x[1])));

        foreach (var chunk in input[1..].ChunkByNonEmpty())
        {
            Map map = [];
            _maps.Add(map);
            foreach (var line in chunk.Skip(1))
            {
                var matches = Utils.NumberPattern().Matches(line).ParseMany<long>().ToArray();
                var mapping = new Mapping(matches[0], matches[1], matches[2]);
                map.Add(mapping);
            }
        }
    }

    public object SolvePart1() => _seeds.Min(seed => _maps.Aggregate(seed, MapTo));

    public object SolvePart2() => _maps
        .Aggregate(_seedRanges, (current, map) => MapRangesTo(new Stack<Vec2DLong>(current), map))
        .Min(r => r.X);

    private static long MapTo(long value, Map map)
    {
        foreach (var mapping in map.Where(mapping => mapping.IsInRange(value)))
            return value + mapping.Offset;
        return value;
    }

    private static List<Vec2DLong> MapRangesTo(Stack<Vec2DLong> ranges, Map map)
    {
        List<Vec2DLong> mapped = [];
        while (ranges.Count > 0)
            MapRangeTo(ranges.Pop(), map, ranges, mapped);
        return mapped;
    }

    private static void MapRangeTo(Vec2DLong range, Map map, Stack<Vec2DLong> ranges, List<Vec2DLong> mapped)
    {
        foreach (var mapping in map)
        {
            if (!mapping.HasRangeOverlap(range, out var overlapStart, out var overlapEnd)) continue;

            var mappedRange = new Vec2DLong(overlapStart + mapping.Offset, overlapEnd + mapping.Offset);
            mapped.Add(mappedRange);

            if (overlapStart > range.X) ranges.Push(range with { Y = overlapStart });

            if (range.Y > overlapEnd) ranges.Push(range with { X = overlapEnd });
            return;
        }

        mapped.Add(range);
    }

    private class Map : List<Mapping>;

    private readonly record struct Mapping(long Destination, long Source, long Length)
    {
        public long Offset => Destination - Source;

        public bool IsInRange(long value) => value >= Source && value < Source + Length;

        public bool HasRangeOverlap(Vec2DLong range, out long overlapStart, out long overlapEnd)
        {
            overlapStart = Math.Max(range.X, Source);
            overlapEnd = Math.Min(range.Y, Source + Length);
            return overlapStart < overlapEnd;
        }
    }
}