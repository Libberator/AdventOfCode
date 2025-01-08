using System;
using System.Linq;
using System.Text.RegularExpressions;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2023.D01;

public class Solution : ISolver
{
    private const string NumberPattern = @"\d";
    private const string WordPattern = @"(\d|(?<=o)ne|(?<=t)wo|(?<=t)hree|four|five|six|seven|(?<=e)ight|nine)";
    private string[] _input = [];

    public void Setup(string[] input) => _input = input;

    public object SolvePart1() => _input.Sum(line => LineValue(line, NumberPattern, int.Parse));

    public object SolvePart2() => _input.Sum(line => LineValue(line, WordPattern, ParseNumber));

    private static int LineValue(string line, string pattern, Func<string, int> parseFunc)
    {
        var matches = Regex.Matches(line, pattern);
        if (matches.Count == 0) return 0;
        return 10 * parseFunc(matches[0].Value) + parseFunc(matches[^1].Value);
    }

    // Words are cut off due to Regex's positive lookbehind to ensure it captures the last word in
    // "tw[o]ne", "eigh[t]wo", "eigh[t]hree", "nin[e]ight" and not only the first word
    private static int ParseNumber(string line) => line.Length == 1
        ? line[0].AsDigit()
        : line switch
        {
            "ne" => 1, "wo" => 2, "hree" => 3,
            "four" => 4, "five" => 5, "six" => 6,
            "seven" => 7, "ight" => 8, "nine" => 9,
            _ => 0
        };
}