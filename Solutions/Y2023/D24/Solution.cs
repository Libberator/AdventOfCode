using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D24;

public class Solution : ISolver
{
    private readonly List<Hailstone> _hailstones = [];

    [TestValue(27)] private readonly long _max = 400000000000000;

    [TestValue(7)] private readonly long _min = 200000000000000;

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split('@');
            var position = Vec3DLong.Parse(split[0]);
            var velocity = Vec3D.Parse(split[1]);
            _hailstones.Add(new Hailstone(position, velocity));
        }
    }

    public object SolvePart1()
    {
        var total = 0;

        for (var i = 0; i < _hailstones.Count - 1; i++)
        {
            var a = _hailstones[i];
            for (var j = i + 1; j < _hailstones.Count; j++)
                if (PathsCrossInWindow(a, _hailstones[j], _min, _max))
                    total++;
        }

        return total;
    }

    public object SolvePart2() => (long)Math.Round(GetStartingCondition(_hailstones).Take(3).Sum());

    private static bool PathsCrossInWindow(Hailstone a, Hailstone b, long min, long max)
    {
        var determinant = a.Vel.X * b.Vel.Y - a.Vel.Y * b.Vel.X;
        if (determinant == 0) return false; // parallel lines

        double quotient = b.Vel.Y * (b.Pos.X - a.Pos.X) - b.Vel.X * (b.Pos.Y - a.Pos.Y);
        var t = quotient / determinant;
        if (t < 0) return false; // checks if a's time was negative

        var x = a.Pos.X + t * a.Vel.X;
        if (x < min || x > max) return false;
        if (Math.Sign(b.Vel.X) != Math.Sign(x - b.Pos.X)) return false; // checks if b's time was negative

        var y = a.Pos.Y + t * a.Vel.Y;
        return y >= min && y <= max;
    }

    private static decimal[] GetStartingCondition(List<Hailstone> hailstones)
    {
        // We have 6 unknown variables (3 for rock position, 3 for rock speed), so we set up 6 linear equations
        // After lots of math and simplifying, we end up with the following equations:
        // 
        // (dy`-dy) X + (dx-dx`) Y + (y-y`) DX + (x`-x) DY =  x` dy` - y` dx` - x dy + y dx
        // (dz`-dz) X + (dx-dx`) Z + (z-z`) DX + (x`-x) DZ =  x` dz` - z` dx` - x dz + z dx
        // (dz-dz`) Y + (dy`-dy) Z + (z`-z) DY + (y-y`) DZ = -y` dz` + z` dy` + y dz - z dy
        //
        // The unknowns for the ROCK are X,Y,Z and DX,DY,DZ for position and velocity respectively
        // A backtick just means a `different` hailstone, not a derivative.
        // Choose any 3 hailstones for 2 unique pairings (e.g. A,B,C makes AB and AC). 3 eqs * 2 pairs = 6 total eqs

        var coefficients = new decimal[6, 6];
        var constants = new decimal[6];

        FillXyRow(coefficients, constants, 0, hailstones[0], hailstones[1]);
        FillXyRow(coefficients, constants, 1, hailstones[0], hailstones[2]);

        FillXzRow(coefficients, constants, 2, hailstones[0], hailstones[1]);
        FillXzRow(coefficients, constants, 3, hailstones[0], hailstones[2]);

        FillYzRow(coefficients, constants, 4, hailstones[0], hailstones[1]);
        FillYzRow(coefficients, constants, 5, hailstones[0], hailstones[2]);

        return Utils.SolveLinearEquations(coefficients, constants);
    }

    private static void FillXyRow(decimal[,] coefficients, decimal[] constants, int row, Hailstone a, Hailstone b)
    {
        // (dy`-dy) X + (dx-dx`) Y + (y-y`) DX + (x`-x) DY =  x` dy` - y` dx` - x dy + y dx
        coefficients[row, 0] = b.Vel.Y - a.Vel.Y;
        coefficients[row, 1] = a.Vel.X - b.Vel.X;
        coefficients[row, 3] = a.Pos.Y - b.Pos.Y;
        coefficients[row, 4] = b.Pos.X - a.Pos.X;
        constants[row] = b.Pos.X * b.Vel.Y - b.Pos.Y * b.Vel.X - a.Pos.X * a.Vel.Y + a.Pos.Y * a.Vel.X;
    }

    private static void FillXzRow(decimal[,] coefficients, decimal[] constants, int row, Hailstone a, Hailstone b)
    {
        // (dz`-dz) X + (dx-dx`) Z + (z-z`) DX + (x`-x) DZ =  x` dz` - z` dx` - x dz + z dx
        coefficients[row, 0] = b.Vel.Z - a.Vel.Z;
        coefficients[row, 2] = a.Vel.X - b.Vel.X;
        coefficients[row, 3] = a.Pos.Z - b.Pos.Z;
        coefficients[row, 5] = b.Pos.X - a.Pos.X;
        constants[row] = b.Pos.X * b.Vel.Z - b.Pos.Z * b.Vel.X - a.Pos.X * a.Vel.Z + a.Pos.Z * a.Vel.X;
    }

    private static void FillYzRow(decimal[,] coefficients, decimal[] constants, int row, Hailstone a, Hailstone b)
    {
        // (dz-dz`) Y + (dy`-dy) Z + (z`-z) DY + (y-y`) DZ = -y` dz` + z` dy` + y dz - z dy
        coefficients[row, 1] = a.Vel.Z - b.Vel.Z;
        coefficients[row, 2] = b.Vel.Y - a.Vel.Y;
        coefficients[row, 4] = b.Pos.Z - a.Pos.Z;
        coefficients[row, 5] = a.Pos.Y - b.Pos.Y;
        constants[row] = -b.Pos.Y * b.Vel.Z + b.Pos.Z * b.Vel.Y + a.Pos.Y * a.Vel.Z - a.Pos.Z * a.Vel.Y;
    }

    private record struct Hailstone(Vec3DLong Pos, Vec3D Vel);
}