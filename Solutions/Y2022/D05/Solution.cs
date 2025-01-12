using System;
using System.Collections.Generic;
using System.Text;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2022.D05;

public class Solution : ISolver
{
    private readonly List<Move> _instructions = [];
    private readonly Dictionary<int, Stack<char>> _stacks = [];

    public void Setup(string[] input)
    {
        var emptyIndex = Array.IndexOf(input, "");
        var stackCount = input[emptyIndex - 1][^2].AsDigit();

        for (var i = 1; i <= stackCount; i++) _stacks[i] = [];

        // Crate chars are found on indices: 1, 5, 9, 13, 17, 21, 25, 29, 33
        foreach (var line in input[..(emptyIndex - 1)])
            for (var i = 1; i < line.Length; i += 4)
                if (line[i] != ' ')
                    _stacks[(i - 1) / 4 + 1].Push(line[i]);

        foreach (var line in input[(emptyIndex + 1)..])
        {
            var matches = Utils.NumberPattern().Matches(line);
            _instructions.Add(new Move(int.Parse(matches[0].ValueSpan), int.Parse(matches[1].ValueSpan),
                int.Parse(matches[2].ValueSpan)));
        }
    }

    public object SolvePart1()
    {
        var copy = MakeReverseCopy(_stacks);
        _instructions.ForEach(move => ApplyMove(copy, move));
        return TopCrates(copy);
    }

    public object SolvePart2()
    {
        var copy = MakeReverseCopy(_stacks);
        _instructions.ForEach(move => ApplyMoveMaintainOrder(copy, move));
        return TopCrates(copy);
    }

    private static void ApplyMove(Dictionary<int, Stack<char>> source, Move move)
    {
        for (var i = 0; i < move.Amount; i++)
            source[move.To].Push(source[move.From].Pop());
    }

    private static void ApplyMoveMaintainOrder(Dictionary<int, Stack<char>> source, Move move)
    {
        var temp = new Stack<char>();
        for (var i = 0; i < move.Amount; i++)
            temp.Push(source[move.From].Pop());
        while (temp.Count != 0)
            source[move.To].Push(temp.Pop());
    }

    private static Dictionary<int, Stack<char>> MakeReverseCopy(Dictionary<int, Stack<char>> source)
    {
        var copy = new Dictionary<int, Stack<char>>();
        foreach (var kvp in source)
            copy[kvp.Key] = new Stack<char>(kvp.Value); // this correctly *reverses* the order for the stack copy
        return copy;
    }

    private static string TopCrates(Dictionary<int, Stack<char>> source)
    {
        var result = new StringBuilder();
        foreach (var kvp in source)
            if (kvp.Value.Count > 0)
                result.Append(kvp.Value.Peek());
        return result.ToString();
    }

    private readonly record struct Move(int Amount, int From, int To);
}