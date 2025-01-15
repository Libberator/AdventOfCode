using System.Collections.Generic;

namespace AoC.Solutions.Y2022.D21;

public class Solution : ISolver
{
    private readonly Dictionary<string, Monkey> _monkeys = new();
    private Monkey? _humn, _root;

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            string?[] split = line.Split(": ");
            var id = split[0]!;
            var job = split[1]!;
            var monkey = decimal.TryParse(job, out var value) ? new Monkey(value) : new Monkey(job[5], job[..4], job[^4..]);
            _monkeys[id] = monkey;
        }
        _humn = _monkeys["humn"];
        _root = _monkeys["root"];
    }

    public object SolvePart1() => _root!.GetValue(_monkeys);

    public object SolvePart2()
    {
        _root!.Operation = '-'; // equality (=) is just subtraction and comparing against 0

        var x0 = _humn!.Value;
        var y0 = _root.GetValue(_monkeys);
        var x1 = x0 + y0;
        decimal y1 = 1;

        while (y1 != 0)
        {
            _humn.Value = x1;
            y1 = _root.GetValue(_monkeys);
            var slope = (y1 - y0) / (x1 - x0);
            (x0, x1) = (x1, x0 - y0 / slope); // x1 = x0 - f(x0) / f'(x0)
            y0 = y1;
        }

        return x0;
    }

    private class Monkey
    {
        private readonly string _left = "", _right = "";
        public char Operation;
        public decimal Value;

        public Monkey(decimal value) => Value = value;

        public Monkey(char operation, string left, string right)
        {
            Operation = operation;
            _left = left;
            _right = right;
        }

        public decimal GetValue(Dictionary<string, Monkey> lookup) => Operation switch
        {
            '+' => lookup[_left].GetValue(lookup) + lookup[_right].GetValue(lookup),
            '-' => lookup[_left].GetValue(lookup) - lookup[_right].GetValue(lookup),
            '*' => lookup[_left].GetValue(lookup) * lookup[_right].GetValue(lookup),
            '/' => lookup[_left].GetValue(lookup) / lookup[_right].GetValue(lookup),
            _ => Value
        };
    }
}