using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D17;

public class Solution : ISolver
{
    private long[] _instructions = [];
    private long _registerA;

    public void Setup(string[] input)
    {
        _registerA = long.Parse(input[0].Split()[^1]);
        _instructions = input[^1].Split()[^1].Split(',').ParseLongs();
    }

    public object SolvePart1() => string.Join(',', RunProgram(_instructions, _registerA));

    public object SolvePart2() => ReverseSearch(_instructions).FirstOrDefault();

    private static List<long> RunProgram(long[] instructions, long regA, long regB = 0, long regC = 0)
    {
        List<long> output = [];
        for (var i = 0; i < instructions.Length - 1; i += 2)
            ProcessOpcode(instructions[i], instructions[i + 1], AsCombo(instructions[i + 1]), ref i);

        return output;

        void ProcessOpcode(long opcode, long literal, long combo, ref int i)
        {
            switch (opcode)
            {
                case 0: regA >>= (int)combo; break; // adv = A Divide by Value
                case 1: regB ^= literal; break; // bxl = B XOR Literal
                case 2: regB = combo & 0b111; break; // bst = B Set Tiny(?)
                case 3: i = regA != 0 ? (int)literal - 2 : i; break; // jnz = Jump if Not Zero
                case 4: regB ^= regC; break; // bxc = B XOR C
                case 5: output.Add(combo & 0b111); break; // out = Output
                case 6: regB = regA >> (int)combo; break; // bdv = B Divide by Value
                case 7: regC = regA >> (int)combo; break; // cdv = C Divide by Value
            }
        }

        long AsCombo(long literal) => literal switch
        {
            4 => regA,
            5 => regB,
            6 => regC,
            _ => literal
        };
    }

    private static IEnumerable<long> ReverseSearch(long[] instructions, long value = 0, int depth = 0)
    {
        if (depth >= instructions.Length) yield break;
        value <<= 3;
        for (var i = 0; i < 8; i++)
        {
            if (!RunProgram(instructions, value + i).SequenceEqual(instructions[^(depth + 1)..])) continue;
            if (depth == instructions.Length - 1) yield return value + i;
            foreach (var result in ReverseSearch(instructions, value + i, depth + 1)) yield return result;
        }
    }
}