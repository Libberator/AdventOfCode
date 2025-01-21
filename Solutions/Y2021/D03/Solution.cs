using System.Linq;
using AoC.Utilities.Extensions;
// ReSharper disable AccessToModifiedClosure

namespace AoC.Solutions.Y2021.D03;

public class Solution : ISolver
{
    private const char One = '1';
    private string[] _data = [];

    public void Setup(string[] input) => _data = input;

    public object SolvePart1()
    {
        var numBits = _data[0].Length;
        var onesFrequency = new int[numBits];
        foreach (var line in _data)
            for (var i = 0; i < numBits; i++)
                onesFrequency[i] += line[i] == One ? 1 : 0;

        var threshold = _data.Length / 2;
        var gammaRate = 0;
        var epsilonRate = 0;
        for (var i = 0; i < numBits; i++)
            if (onesFrequency[i] > threshold)
                gammaRate += 1 << (numBits - i - 1);
            else
                epsilonRate += 1 << (numBits - i - 1);

        return gammaRate * epsilonRate;
    }

    public object SolvePart2()
    {
        var numBits = _data[0].Length;
        var sorted = _data.Order().ToList();
        var oneIndex = Utils.BinarySearch(0, sorted.Count - 1, i => sorted[i][0] == One); // stop at first '1'

        var (majority, minority) = oneIndex > sorted.Count / 2
            ? (sorted[..oneIndex], sorted[oneIndex..]) // give majority to oxygen
            : (sorted[oneIndex..], sorted[..oneIndex]);

        for (var bit = 1; bit < numBits; bit++)
        {
            if (majority.Count > 1)
            {
                oneIndex = Utils.BinarySearch(0, majority.Count - 1, i => majority[i][bit] == One);
                majority = oneIndex > majority.Count / 2 ? majority[..oneIndex] : majority[oneIndex..];
            }

            if (minority.Count > 1)
            {
                oneIndex = Utils.BinarySearch(0, minority.Count - 1, i => minority[i][bit] == One);
                minority = oneIndex > minority.Count / 2 ? minority[oneIndex..] : minority[..oneIndex];
            }
        }

        var oxygenRating = majority[^1].ToBase10(2);
        var scrubberRating = minority[0].ToBase10(2);

        return oxygenRating * scrubberRating;
    }
}