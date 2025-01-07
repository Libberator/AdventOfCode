using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AoC.Solutions;

/// <summary>Interface all solutions will implement.</summary>
public interface ISolver
{
    public void Setup(string[] input);
    public object? SolvePart1();
    public object? SolvePart2();
}

public static class SolverFactory
{
    public static int Year(this ISolver solver) => int.Parse(solver.GetType().Namespace![^8..^4]);
    public static int Day(this ISolver solver) => int.Parse(solver.GetType().Namespace![^2..]);

    public static IEnumerable<ISolver> CreateAllSolvers(bool useTestValues = false)
    {
        var assembly = typeof(ISolver).Assembly;
        foreach (var definedType in assembly.DefinedTypes)
        {
            if (definedType.IsInterface || !typeof(ISolver).IsAssignableFrom(definedType)) continue;
            if (Activator.CreateInstance(definedType) is not ISolver solver) continue;
            if (useTestValues) solver.ApplyTestValues();
            yield return solver;
        }
    }

    public static IEnumerable<ISolver> CreateSolvers(int year, bool useTestValues = false)
    {
        for (var day = 1; day <= 25; day++)
        {
            if (!TryCreateSolver(year, day, out var solver)) continue;
            if (useTestValues) solver.ApplyTestValues();
            yield return solver;
        }
    }

    public static bool TryCreateSolver(int year, int day, [NotNullWhen(true)] out ISolver? solver)
    {
        solver = CreateSolver(year, day);
        return solver != null;
    }

    private static ISolver? CreateSolver(int year, int day)
    {
        var assembly = typeof(ISolver).Assembly;
        var nameSpace = $"{typeof(ISolver).Namespace}.Y{year}.D{day:D2}";
        var fullyQualifiedName = $"{nameSpace}.Solution";
        var type = assembly.GetType(fullyQualifiedName);

        return type != null ? Activator.CreateInstance(type) as ISolver : null;
    }
}