using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D11;

public class Solution : ISolver
{
    private string[] _data = [];

    public void Setup(string[] input) => _data = input[0].Split();

    public object SolvePart1() => BlinkNTimes(25);

    public object SolvePart2() => BlinkNTimes(75);

    private long BlinkNTimes(int n)
    {
        // initialize and copy the source data to the even counts
        Dictionary<string, long> even = [], odd = [];
        foreach (var number in _data)
            even.AddToExistingOrCreate(number, 1);

        for (var i = 0; i < n; i++)
            if (i % 2 == 0) Blink(even, odd);
            else Blink(odd, even);

        return n % 2 == 0 ? even.Sum(kvp => kvp.Value) : odd.Sum(kvp => kvp.Value);
    }

    private static void Blink(Dictionary<string, long> from, Dictionary<string, long> to)
    {
        to.Clear();
        foreach (var (id, qty) in from)
            if (id == "0")
            {
                to.AddToExistingOrCreate("1", qty);
            }
            else if (id.Length % 2 == 0)
            {
                to.AddToExistingOrCreate(id[..(id.Length / 2)], qty);
                var secondHalf = id[(id.Length / 2)..].TrimStart('0');
                if (secondHalf.Length == 0) secondHalf = "0";
                to.AddToExistingOrCreate(secondHalf, qty);
            }
            else
            {
                to.AddToExistingOrCreate((long.Parse(id) * 2024).ToString(), qty);
            }
    }
}