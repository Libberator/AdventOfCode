using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2022.D11;

public class Solution : ISolver
{
    // item starting values and their holder (which monkey ID is holding them) - these two get copied
    private readonly Dictionary<int, Stack<long>> _monkeyItems = [];
    private readonly List<Monkey> _monkeys = [];

    public void Setup(string[] input)
    {
        var pattern = Utils.NumberPattern();
        foreach (var chunk in input.ChunkByNonEmpty())
        {
            var id = chunk[0][^2].AsDigit();
            var startingItems = chunk[1].ParseLongs();
            _monkeyItems[id] = new Stack<long>(startingItems);

            var opMatch = pattern.Match(chunk[2]);
            var opValue = opMatch.Success ? long.Parse(opMatch.ValueSpan) : 0;
            Func<long, long> operation = !opMatch.Success ? old => old * old
                : chunk[2].Contains('*') ? old => old * opValue : old => old + opValue;

            var divisor = int.Parse(pattern.Match(chunk[3]).ValueSpan);
            var trueTarget = chunk[4][^1].AsDigit();
            var falseTarget = chunk[5][^1].AsDigit();

            var monkey = new Monkey(operation, divisor, trueTarget, falseTarget);
            _monkeys.Add(monkey);
        }
    }

    public object SolvePart1() => GetMonkeyBusiness(20, value => value / 3);

    public object SolvePart2()
    {
        var gcd = _monkeys.Select(m => m.Divisor).Product();
        return GetMonkeyBusiness(10000, value => value % gcd);
    }

    private long GetMonkeyBusiness(int rounds, Func<long, long> postModifier)
    {
        Dictionary<int, Stack<long>> monkeyItems = [];
        foreach (var (id, held) in _monkeyItems)
            monkeyItems[id] = new Stack<long>(held); // make a copy

        var inspectionTotals = new long[_monkeys.Count];
        RunRounds(rounds, monkeyItems, inspectionTotals, postModifier);

        return inspectionTotals.OrderDescending().Take(2).Product();
    }

    private void RunRounds(int rounds, Dictionary<int, Stack<long>> items, long[] totals, Func<long, long> postModifier)
    {
        for (var round = 0; round < rounds; round++)
            RunRound(items, totals, postModifier);
    }

    private void RunRound(Dictionary<int, Stack<long>> monkeyItems, long[] totals, Func<long, long> postModifier)
    {
        foreach (var (id, stack) in monkeyItems)
        {
            var monkey = _monkeys[id];
            while (stack.Count > 0)
            {
                totals[id]++;
                var value = postModifier(monkey.Operation(stack.Pop()));
                var target = value % monkey.Divisor == 0 ? monkey.TrueTarget : monkey.FalseTarget;
                monkeyItems[target].Push(value);
            }
        }
    }

    private class Monkey(Func<long, long> operation, int divisor, int trueTarget, int falseTarget)
    {
        public readonly int Divisor = divisor;
        public readonly int FalseTarget = falseTarget;
        public readonly Func<long, long> Operation = operation;
        public readonly int TrueTarget = trueTarget;
    }
}