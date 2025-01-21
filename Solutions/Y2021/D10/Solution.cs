using System.Collections.Generic;

namespace AoC.Solutions.Y2021.D10;

public class Solution : ISolver
{
    private static readonly Dictionary<char, (char MatchingBrace, int Points)> BraceLookup = new()
    {
        // part 1
        [')'] = ('(', 3),
        [']'] = ('[', 57),
        ['}'] = ('{', 1197),
        ['>'] = ('<', 25137),
        // part 2
        ['('] = (')', 1),
        ['['] = (']', 2),
        ['{'] = ('}', 3),
        ['<'] = ('>', 4)
    };

    private string[] _data = [];

    public void Setup(string[] input) => _data = input;

    public object SolvePart1()
    {
        var sum = 0;
        Stack<char> stack = new();
        foreach (var line in _data)
            if (HasError(line, stack, out var wrongBrace))
                sum += BraceLookup[wrongBrace].Points;

        return sum; // 341823
    }

    public object SolvePart2()
    {
        var stack = new Stack<char>();
        var scores = new SortedList<long, long>();

        foreach (var line in _data)
        {
            if (HasError(line, stack, out _)) continue;

            long score = 0;
            while (stack.Count != 0)
            {
                score *= 5;
                score += BraceLookup[stack.Pop()].Points;
            }

            scores.Add(score, score);
        }

        return scores.GetKeyAtIndex(scores.Count / 2);
    }

    private static bool HasError(string line, Stack<char> stack, out char wrongBrace)
    {
        stack.Clear();
        foreach (var brace in line)
            if (IsOpeningBrace(brace))
            {
                stack.Push(brace);
            }
            else if (!ClosingBraceMatches(brace, stack.Pop()))
            {
                wrongBrace = brace;
                return true;
            }

        wrongBrace = '\0';
        return false;

        static bool IsOpeningBrace(char brace) => brace is '(' or '[' or '{' or '<';
        static bool ClosingBraceMatches(char brace, char openBrace) => BraceLookup[brace].MatchingBrace == openBrace;
    }
}