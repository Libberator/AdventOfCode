using System;
using System.Linq;
using System.Text;

namespace AoC.Solutions.Y2022.D25;

public class Solution : ISolver
{
    private string[] _input = [];

    public void Setup(string[] input) => _input = input;

    public object SolvePart1() => Base10ToSnafu(_input.Sum(SnafuToBase10));

    private static long SnafuToBase10(string input)
    {
        long result = 0;
        long multiplier = 1;
        for (var i = input.Length - 1; i >= 0; i--)
        {
            result += multiplier * ToBase10(input[i]);
            multiplier *= 5;
        }

        return result;

        static int ToBase10(char c) => c switch
        {
            '=' => -2,
            '-' => -1,
            _ => c - '0'
        };
    }

    private static string Base10ToSnafu(long value)
    {
        var sb = new StringBuilder();
        var snafuLength = (int)Math.Ceiling(Math.Log(value, 5));
        for (var power = snafuLength; power >= 0; power--)
        {
            var place = (long)Math.Pow(5, power);
            // threshold for follow-up values to sum to 0. 2,12,62... https://oeis.org/A125831
            var bounds = (place - 1) / 2; 
            for (var i = 2; i >= -2; i--)
            {
                var toSubtract = place * i;
                if (value - toSubtract > bounds || value - toSubtract < -bounds) continue;

                value -= toSubtract;
                if (i != 0 || sb.Length != 0) // ignores leading 0's
                    sb.Append(ToSnafu(i));
                break;
            }
        }

        return sb.ToString();

        static char ToSnafu(int i) => i switch
        {
            -2 => '=',
            -1 => '-',
            _ => (char)('0' + i)
        };
    }
}