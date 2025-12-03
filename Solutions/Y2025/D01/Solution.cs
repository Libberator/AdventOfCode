using System;
using System.Collections.Generic;

namespace AoC.Solutions.Y2025.D01;

public class Solution : ISolver
{
    private readonly List<(char Dir, int Value)> _data = [];
    private const int DialStart = 50;
    private const int DialLength = 100;

    public void Setup(string[] input)
    {
        foreach (var line in input)
            _data.Add((line[0], int.Parse(line[1..])));
    }

    public object SolvePart1()
    {
        var dial = DialStart;
        var zeroCount = 0;
        foreach (var (dir, i) in _data)
        {
            dial += dir == 'R' ? i : -i;
            if (dial % DialLength == 0) zeroCount++;
        }

        return zeroCount;
    }

    public object SolvePart2()
    {
        var dial = DialStart;
        var zeroPasses = 0;
        foreach (var (dir, i) in _data)
        {
            var prevDial = dial;
            dial += dir == 'R' ? i % DialLength : -i % DialLength;
            zeroPasses += i / DialLength;
            if (dial is 0 or >= DialLength or <= -DialLength || (prevDial != 0 && prevDial < 0 != dial < 0)) zeroPasses++;
            dial %= DialLength;
        }

        return  zeroPasses;
    }
}