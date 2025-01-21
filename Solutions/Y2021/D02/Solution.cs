using System.Collections.Generic;

namespace AoC.Solutions.Y2021.D02;

public class Solution : ISolver
{
    private const string Forward = "forward", Down = "down", Up = "up";
    private readonly List<(string Dir, int Units)> _data = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split();
            _data.Add((split[0], int.Parse(split[1])));
        }
    }

    public object SolvePart1()
    {
        var horiz = 0;
        var depth = 0;

        foreach (var (dir, units) in _data)
            switch (dir)
            {
                case Forward:
                    horiz += units;
                    break;
                case Down:
                    depth += units;
                    break;
                case Up:
                    depth -= units;
                    break;
            }

        return horiz * depth;
    }

    public object SolvePart2()
    {
        var horiz = 0;
        var depth = 0;
        var aim = 0;

        foreach (var (dir, units) in _data)
            switch (dir)
            {
                case Forward:
                    horiz += units;
                    depth += aim * units;
                    break;
                case Down:
                    aim += units;
                    break;
                case Up:
                    aim -= units;
                    break;
            }

        return horiz * depth;
    }
}