using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2023.D15;

public class Solution : ISolver
{
    private const char Dash = '-'; // EqualsSign = '=';
    private string[] _instructions = [];

    public void Setup(string[] input) => _instructions = input[0].Split(',');

    public object SolvePart1() => _instructions.Sum(HashFunction);

    public object SolvePart2()
    {
        var lensBoxes = Enumerable.Range(0, 256).Select(_ => new List<Lens>()).ToArray();
        PerformInstructions(_instructions, lensBoxes);
        return GetFocusingPower(lensBoxes);
    }

    private static int HashFunction(string label) => label.Aggregate(0, (current, c) => ((current + c) * 17) & 255);

    private static void PerformInstructions(string[] instructions, List<Lens>[] lensBoxes)
    {
        foreach (var instr in instructions)
            if (instr[^1] == Dash)
            {
                var label = instr[..^1];
                var boxIndex = HashFunction(label);
                RemoveLens(label, boxIndex, lensBoxes);
            }
            else
            {
                var label = instr[..^2];
                var focus = instr[^1].AsDigit();
                var boxIndex = HashFunction(label);
                AddOrAdjustLens(label, focus, boxIndex, lensBoxes);
            }
    }

    private static void AddOrAdjustLens(string label, int focus, int boxIndex, List<Lens>[] lensBoxes)
    {
        var box = lensBoxes[boxIndex];
        var lens = box.Find(l => l.Label == label);
        if (lens == null)
            box.Add(new Lens(label, focus));
        else
            lens.Focus = focus;
    }

    private static void RemoveLens(string label, int boxIndex, List<Lens>[] lensBoxes) =>
        lensBoxes[boxIndex].RemoveAll(l => l.Label == label);

    private static int GetFocusingPower(List<Lens>[] lensBoxes)
    {
        var total = 0;
        for (var boxIndex = 0; boxIndex < lensBoxes.Length; boxIndex++)
        {
            var box = lensBoxes[boxIndex];
            for (var lensIndex = 0; lensIndex < box.Count; lensIndex++)
            {
                var lens = box[lensIndex];
                total += (boxIndex + 1) * (lensIndex + 1) * lens.Focus;
            }
        }

        return total;
    }

    private class Lens(string label, int focus)
    {
        public readonly string Label = label;
        public int Focus = focus;
    }
}