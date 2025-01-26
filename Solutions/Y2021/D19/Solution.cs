using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2021.D19;

public class Solution : ISolver
{
    private readonly List<Vec3D[]> _scanners = [];
    
    public void Setup(string[] input)
    {
        foreach (var chunk in input.ChunkByNonEmpty())
            _scanners.Add(Array.ConvertAll(chunk[1..], Vec3D.Parse));
    }

    public object SolvePart1()
    {
        // Need relative positions, making all vectors into diffs of `N * (N - 1) / 2` length
        // would need to keep track of vectors (indices) as reference to determine orientation, then pos
        // could dot product be useful? maybe cross?
        
        // how can we shortcut the operations? pigeonhole principle
        // if 12 need to overlap, and there are N coords, we only need to search N - 11 (all but the 12th)
        // at most to get our FIRST match. In other words, count matches: if 12 not possible, exit early
        
        
        foreach (var scanner in _scanners)
        {
            HashSet<int> distances = [];
            /*for (var i = 0; i < scanner.Length - 1; i++)
            {
                for (var j = i + 1; j < scanner.Length; j++)
                {
                    var dist = scanner[i].DistanceManhattan(scanner[j]);
                    if (!distances.Add(dist))
                    {
                        Console.WriteLine($"index {i} and {j} are {dist} apart. Happened already");
                    }
                }
            }*/
            
            
        }

        return "Part 1";
    }

    public object SolvePart2()
    {
        return "Part 2";
    }
}