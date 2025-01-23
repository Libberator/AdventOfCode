using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2021.D14;

public class Solution : ISolver
{
    private readonly Dictionary<string, char> _insertionPairs = new();
    private string _template = "";

    public void Setup(string[] input)
    {
        _template = input[0];
        foreach (var line in input[2..])
            _insertionPairs.Add(line[..2], line[^1]);
    }

    public object SolvePart1() => RunPairInsertions(10);

    public object SolvePart2() => RunPairInsertions(40);

    private long RunPairInsertions(int cycles)
    {
        Dictionary<string, long> pairCounter = new();
        for (var i = 0; i < _template.Length - 1; i++)
            pairCounter.AddToExistingOrCreate(_template[i..(i + 2)], 1);

        for (var i = 0; i < cycles; i++)
        {
            Dictionary<string, long> newPairCounter = new();
            foreach (var (pair, count) in pairCounter)
            {
                var insertion = _insertionPairs[pair];
                newPairCounter.AddToExistingOrCreate($"{pair[0]}{insertion}", count);
                newPairCounter.AddToExistingOrCreate($"{insertion}{pair[1]}", count);
            }

            pairCounter = newPairCounter;
        }

        Dictionary<char, long> charCounter = new() { [_template[^1]] = 1 };
        foreach (var kvp in pairCounter)
            charCounter.AddToExistingOrCreate(kvp.Key[0], kvp.Value);

        return charCounter.Values.Max() - charCounter.Values.Min();
    }
}