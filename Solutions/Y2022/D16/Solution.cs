using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoC.Utilities.Graphs;

namespace AoC.Solutions.Y2022.D16;

public class Solution : ISolver
{
    private const string Start = "AA";
    private readonly List<Valve> _valves = [];
    private Dictionary<(string From, string To), int> _distanceCosts = [];

    public void Setup(string[] input)
    {
        const string pattern = @"Valve (.{2}) has flow rate=(\d+); tunnel(?:s)? lead(?:s)? to valve(?:s)? (.+)";
        Dictionary<string, HashSet<string>> adjacencyList = new();

        foreach (var line in input)
        {
            var match = Regex.Match(line, pattern);
            var valveId = match.Groups[1].Value;
            var flowRate = int.Parse(match.Groups[2].ValueSpan);
            var neighbors = match.Groups[3].Value.Split(", ");
            adjacencyList.Add(valveId, [..neighbors]);
            if (flowRate > 0)
                _valves.Add(new Valve(valveId, 1 << _valves.Count, flowRate));
        }

        _distanceCosts = Graph.FindShortestPathCosts(adjacencyList);
    }

    public object SolvePart1() => FindHighestPressure(30);

    public object SolvePart2() => FindHighestDualPressure(26);

    private int FindHighestPressure(int minutes, string start = Start)
    {
        var maxPressure = int.MinValue;
        Dictionary<int, int> snapshots = new();
        Stack<(string Id, int Opened, int TimeRemaining, int Pressure)> stack = [];
        stack.Push((start, 0, minutes, 0));

        while (stack.Count > 0)
        {
            var (current, opened, timeLeft, pressure) = stack.Pop();
            if (pressure > maxPressure) maxPressure = pressure;

            // prune
            if (MaxTheoreticalPressure(current, opened, timeLeft, pressure) <= maxPressure) continue;
            if (snapshots.TryGetValue(opened, out var prevBest) && pressure <= prevBest) continue;
            snapshots[opened] = pressure;

            // go to next nodes
            foreach (var n in NextNodes(current, opened, timeLeft))
                stack.Push((n.Id, opened | n.IdMask, n.Remaining, pressure + n.Relieved));
        }

        return maxPressure;
    }

    private IEnumerable<(string Id, int IdMask, int Remaining, int Relieved)> NextNodes(string node, int opened,
        int minutes)
    {
        foreach (var (id, idMask, flow) in _valves)
        {
            if ((opened & idMask) != 0) continue; // opened valve already
            var timeRemaining = minutes - _distanceCosts[(node, id)] - 1;
            if (timeRemaining <= 0) continue; // can't benefit from going there 
            var pressureRelieved = flow * timeRemaining;
            yield return (id, idMask, timeRemaining, pressureRelieved);
        }
    }

    private int MaxTheoreticalPressure(string node, int opened, int minutes, int pressure) =>
        pressure + NextNodes(node, opened, minutes).Sum(next => next.Relieved);

    private int FindHighestDualPressure(int minutes, string start = Start)
    {
        var maxPressure = int.MinValue;
        Dictionary<int, int> snapshots = new();
        Stack<(string ANode, string BNode, int Opened, int ARemaining, int BRemaining, int Pressure)> stack = new();
        stack.Push((start, start, 0, minutes, minutes, 0));

        while (stack.Count > 0)
        {
            var (a, b, opened, aTime, bTime, pressure) = stack.Pop();
            if (pressure > maxPressure) maxPressure = pressure;

            // prune
            if (MaxTheoreticalDualPressure(a, b, opened, aTime, bTime, pressure) <= maxPressure) continue;
            if (snapshots.TryGetValue(opened, out var prevBest) && pressure <= prevBest) continue;
            snapshots[opened] = pressure;

            // Prepare next nodes
            var aNeighbors = NextNodes(a, opened, aTime).ToList();
            var bNeighbors = NextNodes(b, opened, bTime).ToList();
            switch (aNeighbors.Count, bNeighbors.Count)
            {
                case (0, 0):
                    continue;
                case (0, _):
                    aNeighbors.Add((a, 0, 0, 0)); // stay still while b continues
                    break;
                case (_, 0):
                    bNeighbors.Add((b, 0, 0, 0)); // stay still while a continues
                    break;
            }

            // go to valid combos of next nodes
            foreach (var (aNode, aMask, aRemaining, aRelieved) in aNeighbors)
                foreach (var (bNode, bMask, bRemaining, bRelieved) in bNeighbors)
                {
                    if (aMask == bMask) continue;
                    stack.Push((aNode, bNode, opened | aMask | bMask, aRemaining, bRemaining,
                        pressure + aRelieved + bRelieved));
                }
        }

        return maxPressure;
    }

    private int MaxTheoreticalDualPressure(string a, string b, int opened, int aTime, int bTime, int pressure)
    {
        var excess = 0;
        foreach (var (id, idMask, flow) in _valves)
        {
            if ((opened & idMask) != 0) continue; // opened valve already
            var time = Math.Max(aTime - _distanceCosts[(a, id)] - 1, bTime - _distanceCosts[(b, id)] - 1);
            excess += flow * time;
        }

        return pressure + excess;
    }

    private record Valve(string Id, int IdMask, int Flow);
}