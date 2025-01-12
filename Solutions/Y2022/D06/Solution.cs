using System;

namespace AoC.Solutions.Y2022.D06;

public class Solution : ISolver
{
    private string _message = "";

    public void Setup(string[] input) => _message = input[0];

    public object SolvePart1() => IndexOfMessageMarker(_message, 4);

    public object SolvePart2() => IndexOfMessageMarker(_message, 14);

    // As a simple one-liner:
    // return Enumerable.Range(0, msg.Length - size).First(i => msg[i..(i + size)].ToHashSet().Count == size) + size;
    private static int IndexOfMessageMarker(string msg, int size)
    {
        var i = size - 1;
        while (i < msg.Length)
            if (WindowHasRepeat(i, i - size + 1, out var duplicateIndex))
                i = duplicateIndex + size;
            else
                return i + 1;

        throw new Exception("No message marker found");

        bool WindowHasRepeat(int end, int start, out int duplicateIndex)
        {
            duplicateIndex = start;
            var state = 0;
            for (var j = end; j >= start; j--)
            {
                var bitmask = 1 << (msg[j] - 'a');
                if ((state & bitmask) > 0)
                {
                    duplicateIndex = j;
                    return true;
                }

                state |= bitmask;
            }

            return false;
        }
    }
}