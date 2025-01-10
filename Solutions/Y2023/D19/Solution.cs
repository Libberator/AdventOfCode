using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D19;

public class Solution : ISolver
{
    private const string Accepted = "A", Rejected = "R", Start = "in";
    private readonly List<Part> _parts = [];
    private readonly Vec2D _range = new(1, 4000);
    private readonly Dictionary<string, Workflow> _workflows = [];

    public void Setup(string[] input)
    {
        var emptyIndex = Array.IndexOf(input, string.Empty);
        ParseWorkflows(input[..emptyIndex]);
        ParseParts(input[(emptyIndex + 1)..]);
    }

    public object SolvePart1() => _parts.Sum(p => IsAcceptedPart(p, _workflows[Start]) ? p.Total : 0);

    public object SolvePart2()
    {
        var partRange = new PartRange(_range, _range, _range, _range);
        List<PartRange> validRanges = [];
        ProcessRange(partRange, _workflows[Start], validRanges);
        return validRanges.Sum(r => r.Total);
    }

    private void ParseWorkflows(string[] input)
    {
        foreach (var line in input)
        {
            var index = line.IndexOf('{');
            var id = line[..index];
            var rules = line[(index + 1)..^1].Split(',');

            var workflow = new Workflow
            {
                Conditions = ParseConditions(rules[..^1]).ToList(),
                Fallback = rules[^1]
            };
            _workflows.Add(id, workflow);
        }
    }

    private static IEnumerable<Condition> ParseConditions(string[] rules)
    {
        foreach (var rule in rules)
        {
            var letter = rule[0];
            var isGreaterThan = rule[1] == '>';
            var split = rule[2..].Split(':');
            var value = int.Parse(split[0]);
            var passResult = split[1];
            yield return new Condition(letter, isGreaterThan, value, passResult);
        }
    }

    private void ParseParts(string[] input)
    {
        foreach (var line in input)
        {
            var matches = Regex.Matches(line, @"\d+");
            var x = int.Parse(matches[0].Value);
            var m = int.Parse(matches[1].Value);
            var a = int.Parse(matches[2].Value);
            var s = int.Parse(matches[3].Value);
            _parts.Add(new Part(x, m, a, s));
        }
    }

    private bool IsAcceptedPart(Part part, Workflow workflow)
    {
        foreach (var condition in workflow.Conditions)
            if (condition.Passes(part))
                return condition.PassResult switch
                {
                    Accepted => true,
                    Rejected => false,
                    _ => IsAcceptedPart(part, _workflows[condition.PassResult])
                };

        return workflow.Fallback switch
        {
            Accepted => true,
            Rejected => false,
            _ => IsAcceptedPart(part, _workflows[workflow.Fallback])
        };
    }

    private void ProcessRange(PartRange partRange, Workflow workflow, List<PartRange> validRanges)
    {
        foreach (var (letter, isGreaterThan, value, passResult) in workflow.Conditions)
        {
            Vec2D failRange, passRange; // For splitting into two ranges
            if (isGreaterThan)
                partRange.Split(letter, value, out failRange, out passRange);
            else
                partRange.Split(letter, value - 1, out passRange, out failRange);

            var partRangeCopy = partRange;
            partRange[letter] = failRange;
            partRangeCopy[letter] = passRange;

            if (passResult == Accepted)
                validRanges.Add(partRangeCopy);
            else if (passResult != Rejected)
                ProcessRange(partRangeCopy, _workflows[passResult], validRanges);
        }

        if (workflow.Fallback == Accepted)
            validRanges.Add(partRange);
        else if (workflow.Fallback != Rejected)
            ProcessRange(partRange, _workflows[workflow.Fallback], validRanges);
    }

    private class Workflow
    {
        public required List<Condition> Conditions { get; init; }
        public required string Fallback { get; init; }
    }

    private readonly record struct Condition(char Letter, bool IsGreaterThan, int Value, string PassResult)
    {
        public bool Passes(Part part) =>
            IsGreaterThan ? part[Letter] > Value : part[Letter] < Value;
    }

    private readonly record struct Part(int X, int M, int A, int S)
    {
        public int Total => X + M + A + S;

        public int this[char letter] => letter switch
        {
            'x' => X, 'm' => M, 'a' => A, 's' => S,
            _ => throw new IndexOutOfRangeException($"Invalid key: {letter}")
        };
    }

    private record struct PartRange(Vec2D X, Vec2D M, Vec2D A, Vec2D S)
    {
        public readonly long Total => Length(X) * Length(M) * Length(A) * Length(S);

        public Vec2D this[char letter]
        {
            readonly get => letter switch
            {
                'x' => X, 'm' => M, 'a' => A, 's' => S,
                _ => throw new IndexOutOfRangeException($"Invalid key: {letter}")
            };
            set
            {
                switch (letter)
                {
                    case 'x': X = value; break;
                    case 'm': M = value; break;
                    case 'a': A = value; break;
                    case 's': S = value; break;
                    default: throw new IndexOutOfRangeException($"Invalid key: {letter}");
                }
            }
        }

        public void Split(char letter, int value, out Vec2D left, out Vec2D right)
        {
            left = this[letter] with { Y = value };
            right = this[letter] with { X = value + 1 };
        }

        private static long Length(Vec2D r) => r.Y - r.X + 1;
    }
}