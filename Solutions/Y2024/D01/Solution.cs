using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D01;

public class Solution : ISolver
{
    private readonly SortedDictionary<int, int> _left = new();
    private readonly SortedDictionary<int, int> _right = new();

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split("   ");
            _left.AddToExistingOrCreate(int.Parse(split[0]), 1);
            _right.AddToExistingOrCreate(int.Parse(split[1]), 1);
        }
    }

    public object SolvePart1()
    {
        var total = 0;

        var rightEnumerator = _right.GetEnumerator();
        var (rightKey, rightValue) = rightEnumerator.Current;

        foreach (var left in _left)
            for (var i = 0; i < left.Value; i++)
            {
                if (rightValue == 0)
                {
                    rightEnumerator.MoveNext();
                    (rightKey, rightValue) = rightEnumerator.Current;
                }

                total += Math.Abs(left.Key - rightKey);
                rightValue--;
            }

        return total;
    }

    public object SolvePart2() => _left.Sum(kvp => kvp.Key * kvp.Value * _right.GetValueOrDefault(kvp.Key, 0));
}