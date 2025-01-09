using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2023.D12;

public class Solution : ISolver
{
    private const char Damaged = '#', Working = '.', Unknown = '?';
    private readonly List<Report> _reports = [];
    private readonly List<Report> _unfoldedReports = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split(' ');
            var condition = split[0].AsMemory();
            var groups = split[1].ParseInts();
            _reports.Add(new Report(condition, groups));

            var unfoldedCondition = string.Join(Unknown, Enumerable.Repeat(condition, 5)).AsMemory();
            var unfoldedGroups = Enumerable.Repeat(groups, 5).SelectMany(arr => arr).ToArray();
            _unfoldedReports.Add(new Report(unfoldedCondition, unfoldedGroups));
        }
    }

    public object SolvePart1() => _reports.Sum(r => Recurse(r, []));

    public object SolvePart2() => _unfoldedReports.Sum(r => Recurse(r, []));

    private static long Recurse(Report report, Dictionary<Report, long> cache)
    {
        if (cache.TryGetValue(report, out var cachedTotal)) return cachedTotal;

        var condition = report.Condition;
        var groups = report.Groups;
        if (groups.Length == 0) return condition.Span.Contains(Damaged) ? 0 : 1; // fail vs success

        var group = groups[0];
        var latestIndex = condition.Length - (groups.Sum() + groups.Length) + 1; // furthest we can slide window

        long total = 0;
        for (var i = 0; i <= latestIndex; i++)
        {
            // check for all the early outs to prune branches
            if (condition.Span[..i].Contains(Damaged)) break; // can't skip over a known damaged spring, '#'
            var endIndex = i + group;
            if (condition[i..endIndex].Span.Contains(Working)) continue; // can't slide this window on a '.'
            if (endIndex >= condition.Length) return total + 1; // this successful group reached end. yay! 
            if (condition.Span[endIndex] == Damaged) continue; // next char is not a '.' or '?'. that's a no-no

            var next = new Report(condition[(endIndex + 1)..].TrimStart(Working),
                groups[1..]); // +1 for spacing between groups
            total += Recurse(next, cache);
        }

        cache.Add(report, total);
        return total;
    }

    private readonly record struct Report(ReadOnlyMemory<char> Condition, int[] Groups)
    {
        public bool Equals(Report other) => Condition.Span.SequenceEqual(other.Condition.Span) &&
                                            StructuralComparisons.StructuralEqualityComparer.Equals(Groups,
                                                other.Groups);

        public override int GetHashCode() => HashCode.Combine(Condition,
            StructuralComparisons.StructuralEqualityComparer.GetHashCode(Groups));
    }
}