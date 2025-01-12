using System;
using System.Text;

namespace AoC.Solutions.Y2022.D10;

public class Solution : ISolver
{
    private const string NoOp = "noop";
    private readonly StringBuilder _crtDisplay = new();
    private int _registerX = 1, _cycle, _totalStrength;

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            IncrementCycle();
            if (line == NoOp) continue;
            IncrementCycle();
            _registerX += int.Parse(line[5..]);
        }
    }

    public object SolvePart1() => _totalStrength;

    public object SolvePart2() => _crtDisplay.ToString().TrimEnd();

    private void IncrementCycle()
    {
        _crtDisplay.Append(Math.Abs(_cycle % 40 - _registerX) <= 1 ? '#' : '.'); // draw pixel
        _cycle++;
        if (_cycle % 40 == 0) _crtDisplay.Append('\n'); // AppendLine() is \r\n for non-Unix systems  
        if ((_cycle + 20) % 40 == 0) _totalStrength += _cycle * _registerX; // 20, 60, 100, 140, ...
    }
}