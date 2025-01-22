using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Algorithms;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D23;

public class Solution : ISolver
{
    private readonly Dictionary<string, HashSet<string>> _graph = [];
    
    public void Setup(string[] input)
    {
        foreach (var pair in input.Select(line => line.Split('-')))
        {
            if (!_graph.TryAdd(pair[0], [pair[1]])) _graph[pair[0]].Add(pair[1]);
            if (!_graph.TryAdd(pair[1], [pair[0]])) _graph[pair[1]].Add(pair[0]);
        }
    }

    public object SolvePart1()
    {
        var count = 0;

        foreach (var (a, abConnections) in _graph)
            foreach (var b in abConnections)
                foreach (var c in abConnections.Intersect(_graph[b]))
                    if (IsValidTriplet(a, b, c))
                        count++;

        return count;
    }

    public object SolvePart2() => Graph.FindMaxClique(_graph).Order().JoinAsString();

    private static bool IsValidTriplet(string a, string b, string c) =>
        string.CompareOrdinal(a, b) < 0 && string.CompareOrdinal(b, c) < 0 &&
        (a.StartsWith('t') || b.StartsWith('t') || c.StartsWith('t'));
}