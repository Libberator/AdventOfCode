using System.Linq;

namespace AoC.Solutions.Y2022.D03;

public class Solution : ISolver
{
    private string[] _data = [];

    public void Setup(string[] input) => _data = input;

    public object SolvePart1() =>
        _data.Sum(l => GetPriority(l[..(l.Length / 2)].Intersect(l[(l.Length / 2)..]).Single()));

    public object SolvePart2()
    {
        var total = 0;
        for (var i = 0; i < _data.Length - 2; i += 3)
        {
            var charInCommon = _data[i].Intersect(_data[i + 1]).Intersect(_data[i + 2]).Single();
            total += GetPriority(charInCommon);
        }

        return total;
    }

    private static int GetPriority(char c) => c switch
    {
        >= 'a' and <= 'z' => c - 'a' + 1, // 1-26
        >= 'A' and <= 'Z' => c - 'A' + 27, // 27-52
        _ => 0
    };
}