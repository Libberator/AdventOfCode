using System.Collections.Generic;

namespace AoC.Solutions.Y2021.D12;

public class Solution : ISolver
{
    private const string Start = "start", End = "end", DoubleVisitFlag = "UsedDoubleVisit";
    private readonly Dictionary<string, HashSet<string>> _adjacencyList = [];
    private readonly Dictionary<string, int> _idMask = [];

    public void Setup(string[] input)
    {
        var id = 0;
        foreach (var line in input)
        {
            var split = line.Split('-');
            var (a, b) = (split[0], split[1]);
            if (!_adjacencyList.TryAdd(a, [b])) _adjacencyList[a].Add(b);
            if (!_adjacencyList.TryAdd(b, [a])) _adjacencyList[b].Add(a);
            if (_idMask.TryAdd(a, 1 << id)) id++;
            if (_idMask.TryAdd(b, 1 << id)) id++;
        }

        _idMask[DoubleVisitFlag] = 1 << id;
    }

    public object SolvePart1() => CountAllPaths(Start, _idMask[Start]);

    public object SolvePart2() => CountAllPaths(Start, _idMask[Start], true);

    private static bool HasVisited(int visitedMask, int nextMask) => (visitedMask & nextMask) == nextMask;
    private static bool IsUppercase(string node) => string.Equals(node, node.ToUpper());

    private int CountAllPaths(string node, int visitedMask, bool useDoubleVisit = false)
    {
        var count = 0;
        foreach (var next in _adjacencyList[node])
        {
            var idMask = _idMask[next];
            switch (next)
            {
                case Start: break;
                case End: count++; break;
                default:
                    if (!HasVisited(visitedMask, idMask) || IsUppercase(next))
                        count += CountAllPaths(next, visitedMask | idMask, useDoubleVisit);
                    else if (useDoubleVisit && !HasVisited(visitedMask, _idMask[DoubleVisitFlag]))
                        count += CountAllPaths(next, visitedMask | _idMask[DoubleVisitFlag], useDoubleVisit);
                    break;
            }
        }

        return count;
    }
}