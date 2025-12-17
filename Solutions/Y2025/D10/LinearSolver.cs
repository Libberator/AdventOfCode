using System;

namespace AoC.Solutions.Y2025.D10;

public static class LinearSolver
{
    // ILP (Integer Linear Programming) Solver
    public static int[] IntegerMinimizationSolver(int[][] coefficients, int[] constants)
    {
        var upperBounds = ComputeUpperBounds(coefficients, constants);
        var remainingMax = ComputeRemainingMax(coefficients, upperBounds);

        var vars = coefficients.Length;
        var eqs = constants.Length;
        int[] best = [];
        var bestSum = int.MaxValue;
        var current = new int[vars];

        Search(0);
        return best;

        void Search(int index)
        {
            if (!IsStillFeasible(index, current, coefficients, constants, remainingMax))
                return;

            // pruning: if we already exceed best, stop
            var partial = 0;
            for (var i = 0; i < index; i++)
                partial += current[i];

            if (partial >= bestSum)
                return;

            if (index == vars)
            {
                // check equations
                for (var eq = 0; eq < eqs; eq++)
                {
                    var sum = 0;
                    for (var v = 0; v < vars; v++)
                        sum += coefficients[v][eq] * current[v];

                    if (sum != constants[eq])
                        return;
                }

                bestSum = partial;
                best = (int[])current.Clone();
                return;
            }

            for (var v = 0; v <= upperBounds[index]; v++)
            {
                current[index] = v;
                Search(index + 1);
            }
        }
    }

    private static int[] ComputeUpperBounds(int[][] coefficients, int[] constants)
    {
        var vars = coefficients.Length;
        var eqs = constants.Length;
        var bounds = new int[vars];

        for (var v = 0; v < vars; v++)
        {
            var bound = int.MaxValue;

            for (var e = 0; e < eqs; e++)
            {
                var coeff = coefficients[v][e];
                if (coeff > 0)
                    bound = Math.Min(bound, constants[e] / coeff);
            }

            if (bound == int.MaxValue)
                throw new ArgumentException(
                    $"Variable {v} does not participate in any equation.");

            bounds[v] = bound;
        }

        return bounds;
    }

    private static int[,] ComputeRemainingMax(int[][] coefficients, int[] upperBounds)
    {
        var vars = coefficients.Length;
        var eqs = coefficients[0].Length;

        var remainingMax = new int[vars + 1, eqs];

        for (var v = vars - 1; v >= 0; v--)
        {
            for (var e = 0; e < eqs; e++)
                remainingMax[v, e] =
                    remainingMax[v + 1, e] +
                    coefficients[v][e] * upperBounds[v];
        }

        return remainingMax;
    }

    private static bool IsStillFeasible(int index, int[] current, int[][] coefficients, int[] constants,
        int[,] remainingMax)
    {
        var eqs = constants.Length;

        for (var e = 0; e < eqs; e++)
        {
            var sum = 0;

            for (var v = 0; v < index; v++)
                sum += coefficients[v][e] * current[v];

            // Prune: exceeded constant
            if (sum > constants[e])
                return false;

            // Prune: cannot reach constant even with max remaining
            if (sum + remainingMax[index, e] < constants[e])
                return false;
        }

        return true;
    }
}