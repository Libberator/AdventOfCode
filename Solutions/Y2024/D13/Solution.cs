using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D13;

public class Solution : ISolver
{
    private static readonly Vec2DLong Part2Offset = 10_000_000_000_000 * Vec2DLong.One;
    private readonly List<Machine> _machines = [];

    public void Setup(string[] input)
    {
        for (var i = 0; i < input.Length; i += 4)
        {
            var a = new Vec2D(int.Parse(input[i + 0][12..14]), int.Parse(input[i + 0][^2..]));
            var b = new Vec2D(int.Parse(input[i + 1][12..14]), int.Parse(input[i + 1][^2..]));
            var matches = Utils.NumberPattern().Matches(input[i + 2]);
            var prize = new Vec2DLong(int.Parse(matches[0].Value), int.Parse(matches[1].Value));

            _machines.Add(new Machine(a, b, prize));
        }
    }

    public object SolvePart1() => _machines.Sum(m => TokensToWin(m));

    public object SolvePart2() => _machines.Sum(m => TokensToWin(m, true));

    private static long TokensToWin(Machine machine, bool part2 = false)
    {
        var (a, b, prize) = machine;
        if (part2) prize += Part2Offset;

        var rhs = prize.X * b.Y - prize.Y * b.X;
        var lhs = a.X * b.Y - a.Y * b.X;
        if (rhs % lhs != 0) return 0; // no solution

        var aPresses = rhs / lhs;
        var bPresses = (prize.X - aPresses * a.X) / b.X;
        return 3 * aPresses + bPresses;
    }

    private record struct Machine(Vec2D A, Vec2D B, Vec2DLong Prize);
}