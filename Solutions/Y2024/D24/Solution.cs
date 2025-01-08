using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D24;

public class Solution : ISolver
{
    private const string And = "AND", Or = "OR", Xor = "XOR";
    private readonly Dictionary<string, Gate> _gates = [];
    private readonly Dictionary<string, bool> _startingStates = [];

    public void Setup(string[] input)
    {
        var chunks = input.ChunkByNonEmpty();

        foreach (var split in chunks[0].Select(l => l.Split(": ")))
            _startingStates[split[0]] = split[1] == "1";

        foreach (var split in chunks[1].Select(l => l.Split(' ')))
            _gates.Add(split[4], new Gate(split[0], split[1], split[2]));
    }

    public object SolvePart1() => _gates.Keys
        .Where(g => g.StartsWith('z') && Evaluate(g, _startingStates))
        .Aggregate(0L, (number, gate) => number | (1L << int.Parse(gate[1..])));

    public object SolvePart2() => Fix(_gates).Order().JoinAsString();

    private bool Evaluate(string node, Dictionary<string, bool> start)
    {
        if (start.TryGetValue(node, out var state)) return state;

        var gate = _gates[node];
        return gate.Op switch
        {
            And => Evaluate(gate.In1, start) & Evaluate(gate.In2, start),
            Or => Evaluate(gate.In1, start) | Evaluate(gate.In2, start),
            Xor => Evaluate(gate.In1, start) ^ Evaluate(gate.In2, start),
            _ => state
        };
    }

    private IEnumerable<string> Fix(Dictionary<string, Gate> gates)
    {
        var carry = Output(gates, "x00", And, "y00");
        for (var i = 1; i < 45; i++)
        {
            string x = $"x{i:D2}", y = $"y{i:D2}", z = $"z{i:D2}";

            var xor1 = Output(gates, x, Xor, y);
            var and1 = Output(gates, x, And, y);
            var xor2 = Output(gates, carry, Xor, xor1);
            var and2 = Output(gates, carry, And, xor1);

            if (string.IsNullOrEmpty(xor2) && string.IsNullOrEmpty(and2))
                return SwapAndFix(gates, xor1, and1);

            carry = Output(gates, and1, Or, and2);
            if (xor2 != z) return SwapAndFix(gates, z, xor2);
        }

        return [];
    }

    private static string Output(Dictionary<string, Gate> gates, string x, string op, string y) => gates
        .FirstOrDefault(kvp =>
            (kvp.Value.In1 == x && kvp.Value.Op == op && kvp.Value.In2 == y) ||
            (kvp.Value.In1 == y && kvp.Value.Op == op && kvp.Value.In2 == x)).Key;

    private IEnumerable<string> SwapAndFix(Dictionary<string, Gate> gates, string out1, string out2)
    {
        if (string.IsNullOrEmpty(out1) || string.IsNullOrEmpty(out2)) return [];
        (gates[out1], gates[out2]) = (gates[out2], gates[out1]);
        return Fix(gates).Concat([out1, out2]);
    }

    private record Gate(string In1, string Op, string In2);
}