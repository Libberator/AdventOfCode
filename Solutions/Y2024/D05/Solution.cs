using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D05;

public class Solution : ISolver
{
    private readonly List<int[]> _pages = [];
    private readonly Dictionary<int, HashSet<int>> _rules = new();

    public void Setup(string[] input)
    {
        var chunks = input.ChunkByNonEmpty();
        foreach (var line in chunks[0])
        {
            var rule = line.ParseInts();
            if (!_rules.TryAdd(rule[0], [rule[1]]))
                _rules[rule[0]].Add(rule[1]);
        }

        foreach (var line in chunks[1])
            _pages.Add(line.ParseInts());
    }

    public object SolvePart1() => _pages.Where(IsSorted).Sum(p => p.Middle());

    public object SolvePart2() => _pages.Where(IsUnsorted).Sum(p => Sort(p).Middle());

    private bool IsUnsorted(int[] pages) => !IsSorted(pages);

    private bool IsSorted(int[] pages)
    {
        for (var i = 0; i < pages.Length - 1; i++)
            if (_rules.TryGetValue(pages[i + 1], out var rule) && rule.Contains(pages[i]))
                return false;

        return true;
    }

    private int[] Sort(int[] pages)
    {
        Array.Sort(pages, (x, y) => _rules.TryGetValue(x, out var rule) && rule.Contains(y) ? -1 : 1);
        return pages;
    }
}