using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2025.D03;

public class Solution : ISolver
{
    private string[] _input = [];

    public void Setup(string[] input) => _input = input;

    public object SolvePart1() => _input.Sum(line => GetJoltage(line, 2));

    public object SolvePart2() => _input.Sum(line => GetJoltage(line, 12));

    private static long GetJoltage(string line, int targetLength)
    {
        var digits = new List<char>(line);

        Loop:
        while (digits.Count > targetLength)
        {
            for (var i = 0; i < digits.Count - 1; i++)
            {
                if (digits[i] >= digits[i + 1]) continue;
                digits.RemoveAt(i);
                goto Loop;
            }

            while (digits.Count > targetLength)
                for (var num = '1'; num <= '9'; num++)
                {
                    var index = digits.IndexOf(num);
                    if (index == -1) continue;
                    digits.RemoveAt(index);
                    break;
                }
        }

        return digits.Aggregate<char, long>(0, (current, c) => current * 10 + c.AsDigit());
    }
}