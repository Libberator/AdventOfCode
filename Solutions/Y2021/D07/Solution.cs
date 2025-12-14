using System;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2021.D07;

public class Solution : ISolver
{
    private int[] _data = [];

    public void Setup(string[] input) => _data = Array.ConvertAll(input[0].Split(','), int.Parse);

    public object SolvePart1()
    {
        var medianValue = _data.Median();
        var cost1 = GetFuelCostAtSpot(_data, medianValue);
        var cost2 = GetFuelCostAtSpot(_data, medianValue - 1); // in case of even number of elements
        return Math.Min(cost1, cost2);
    }

    public object SolvePart2()
    {
        var meanValue = _data.Average();
        var cost1 = GetFuelCostAtSpot(_data, meanValue, Utils.TriangleSum);
        var cost2 = GetFuelCostAtSpot(_data, meanValue + 1, Utils.TriangleSum); // in case rounding down was bad
        return Math.Min(cost1, cost2);
    }

    private static int GetFuelCostAtSpot(int[] data, int spot, Func<int, int>? costFunc = null)
    {
        costFunc ??= d => d;
        return data.Sum(pos => costFunc(Math.Abs(spot - pos)));
    }
}