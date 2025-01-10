using System;
using System.Collections.Generic;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D18;

using Instruction = (Vec2DLong Direction, long Value);

public class Solution : ISolver
{
    private readonly List<Instruction> _hexInstructions = [];
    private readonly List<Instruction> _instructions = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split(' ');
            var letter = split[0][0];
            var number = long.Parse(split[1]);
            var hex = split[2].Trim('(', '#', ')');

            var instruction = new Instruction(AsDirection(letter), number);
            _instructions.Add(instruction);

            var hexInstruction = new Instruction(AsDirection(hex[^1]), Convert.ToInt64(hex[..^1], 16));
            _hexInstructions.Add(hexInstruction);
        }

        return;

        static Vec2DLong AsDirection(char input) => input switch
        {
            'R' or '0' => Vec2DLong.E,
            'D' or '1' => Vec2DLong.S,
            'L' or '2' => Vec2DLong.W,
            'U' or '3' => Vec2DLong.N,
            _ => throw new IndexOutOfRangeException()
        };
    }

    public object SolvePart1() => CalculatePolygonArea(_instructions);

    public object SolvePart2() => CalculatePolygonArea(_hexInstructions);

    // this uses a slightly adjusted shoelace formula: https://en.wikipedia.org/wiki/Shoelace_formula
    private static long CalculatePolygonArea(List<Instruction> instructions)
    {
        long shoelaceArea = 0;
        long perimeter = 0;
        var prev = Vec2DLong.Zero;

        foreach (var instruction in instructions)
        {
            var curr = prev + instruction.Direction * instruction.Value; // next vertex

            shoelaceArea += prev.X * curr.Y - prev.Y * curr.X;
            perimeter += instruction.Value;

            prev = curr;
        }

        shoelaceArea = Math.Abs(shoelaceArea) / 2;
        // we add half the perimeter and 1 because formula doesn't include lower bounds due to off-by-1 integer math
        return shoelaceArea + perimeter / 2 + 1;
    }
}