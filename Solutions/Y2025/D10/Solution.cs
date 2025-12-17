using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2025.D10;

public class Solution : ISolver
{
    private const char Off = '.';
    private const char On = '#';
    private readonly List<Machine> _machines = [];

    public void Setup(string[] input) =>
        _machines.AddRange(input.Select(line =>
        {
            var lights = Regex.Match(line, @"\[(.*)\]").Groups[1].Value;
            var buttons = Regex.Matches(line, @"\((.+?)\)").Select(m => m.Groups[1].Value.ParseInts()).ToList();
            var jolts = Regex.Match(line, @"\{((\d+,*)+)\}").Groups[1].Value.ParseInts();

            return new Machine(lights, buttons, jolts);
        }));

    public object SolvePart1() => _machines.Sum(m => m.GetLightPresses());

    public object SolvePart2()
    {
        var total = 0;
        Parallel.ForEach(_machines, machine =>
        {
            var jolts = machine.GetJoltagePresses();
            Interlocked.Add(ref total, jolts);
        });

        return total;
    }

    private record Machine(string Lights, List<int[]> Buttons, int[] Jolts)
    {
        private string StartingState => '.'.Repeat(Lights.Length);

        public int GetLightPresses() => ButtonPresses(Lights);

        private int ButtonPresses(string target)
        {
            HashSet<string> visited = [];
            Queue<(string State, int Depth)> states = new([(StartingState, 0)]);

            while (states.Count != 0)
            {
                var current = states.Dequeue();
                if (!visited.Add(current.State))
                    continue;

                var depth = current.Depth + 1;
                foreach (var button in Buttons)
                {
                    var next = current.State;
                    foreach (var i in button)
                        next = next.SetAt(i, next[i] == Off ? On : Off);

                    if (next == target) return depth;
                    states.Enqueue((next, depth));
                }
            }

            return 0;
        }

        public int GetJoltagePresses()
        {
            var coefficients = new int[Buttons.Count][];
            for (var i = 0; i < Buttons.Count; i++)
            {
                var buttons = Buttons[i];
                coefficients[i] = new int[Jolts.Length];
                for (var j = 0; j < Jolts.Length; j++)
                    if (buttons.Contains(j))
                        coefficients[i][j] = 1;
            }

            return LinearSolver.IntegerMinimizationSolver(coefficients, Jolts).Sum();
        }
    }
}