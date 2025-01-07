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