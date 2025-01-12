using System.Collections.Generic;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D09;

public class Solution : ISolver
{
    private readonly List<(Vec2D, int)> _moves = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
            _moves.Add((AsDir(line[0]), int.Parse(line[2..])));
    }

    public object SolvePart1() => ApplyMoves(_moves, 2);

    public object SolvePart2() => ApplyMoves(_moves, 10);

    private static int ApplyMoves(List<(Vec2D, int)> moves, int ropeLength)
    {
        var knots = new Vec2D[ropeLength];
        var trail = new HashSet<Vec2D>([Vec2D.Zero]);
        foreach (var (dir, amount) in moves)
            ApplyMove(knots, trail, dir, amount);
        return trail.Count;
    }

    private static void ApplyMove(Vec2D[] knots, HashSet<Vec2D> trail, Vec2D dir, int amount)
    {
        for (var n = 0; n < amount; n++)
        {
            knots[0] += dir; // move the head
            for (var i = 1; i < knots.Length; i++)
            {
                if (knots[i].DistanceChebyshev(knots[i - 1]) <= 1) break;
                knots[i] += (knots[i - 1] - knots[i]).Normalized(); // rest will follow
                if (i == knots.Length - 1) // track tail movements
                    trail.Add(knots[i]);
            }
        }
    }

    private static Vec2D AsDir(char dir) => dir switch
    {
        'U' => Vec2D.N, 'R' => Vec2D.E, 'D' => Vec2D.S, 'L' => Vec2D.W, _ => Vec2D.Zero
    };
}