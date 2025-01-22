using System.Collections.Generic;
using AoC.Utilities.Algorithms;

namespace AoC.Solutions.Y2023.D25;

public class Solution : ISolver
{
    private readonly Dictionary<string, HashSet<string>> _adjacencyList = new();

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split(": ");
            var a = split[0];
            var bNodes = split[1].Split(' ');

            _adjacencyList.TryAdd(a, []);
            foreach (var b in bNodes)
            {
                _adjacencyList[a].Add(b);
                _adjacencyList.TryAdd(b, []);
                _adjacencyList[b].Add(a);
            }
        }
    }

    public object SolvePart1()
    {
        var nodes = new List<string>(_adjacencyList.Keys);
        var source = nodes[0];
        for (var i = 1; i < nodes.Count; i++)
        {
            var sink = nodes[i];
            var (minCut, subgraph) = Graph.GetMaxFlowMinCut(_adjacencyList, source, sink);
            if (minCut > 3) continue;
            return subgraph.Count * (nodes.Count - subgraph.Count);
        }

        return 0;
    }
}