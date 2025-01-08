using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoC.Utilities.Collections;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2023.D04;

public class Solution : ISolver
{
    private const int MaxWinAmount = 10;
    private readonly List<int> _cardMatches = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var match = Regex.Match(line, @".+:([ \d]+)\|([ \d]+)");
            var winningNumbers = match.Groups[1].Value.ParseInts();
            var cardNumbers = match.Groups[2].Value.ParseInts();
            var matches = cardNumbers.Count(n => winningNumbers.Contains(n));
            _cardMatches.Add(matches);
        }
    }

    public object SolvePart1() => _cardMatches.Sum(n => n > 0 ? (int)Math.Pow(2, n - 1) : 0);

    public object SolvePart2()
    {
        var numberOfCards = _cardMatches.Count; // start with 1 of each original Card
        var buffer = new CircularBuffer<int>(MaxWinAmount);

        for (var i = 0; i < _cardMatches.Count; i++)
        {
            var extraCopies = buffer[i];
            numberOfCards += extraCopies;
            buffer[i] = 0;

            var matches = _cardMatches[i];
            for (var j = 0; j < matches; j++)
                buffer[i + 1 + j] += 1 + extraCopies;
        }

        return numberOfCards;
    }
}