using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2022.D02;

public class Solution : ISolver
{
    private readonly Dictionary<string, int> _data = new();

    public void Setup(string[] input)
    {
        foreach (var line in input)
            _data.AddToExistingOrCreate(line, 1);
    }

    public object SolvePart1() => _data.Sum(kvp => kvp.Value * (MatchScore(kvp.Key) + ThrowScore(kvp.Key[^1])));

    public object SolvePart2() => _data.Sum(kvp => kvp.Value * (MatchScoreKnown(kvp.Key[^1]) + UpdateThrow(kvp.Key)));

    private static int MatchScore(string match) => match switch
    {
        "A Y" or "B Z" or "C X" => 6, // Win
        "A X" or "B Y" or "C Z" => 3, // Tie
        _ => 0 // Loss in all other cases
    };

    private static int ThrowScore(char myThrow) => myThrow switch
    {
        'X' => 1, 'Y' => 2, 'Z' => 3, _ => 0
    };

    private static int MatchScoreKnown(char result) => 3 * (ThrowScore(result) - 1);

    private static int UpdateThrow(string match) => match switch
    {
        "A Y" or "B X" or "C Z" => 1,
        "A Z" or "B Y" or "C X" => 2,
        "A X" or "B Z" or "C Y" => 3,
        _ => 0
    };
}