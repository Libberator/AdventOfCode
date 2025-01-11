using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2023.D20;

public class Solution : ISolver
{
    // Parsing input
    private const char FlipFlopChar = '%', ConjunctionChar = '&';
    private const string StartId = "broadcaster";
    [TestValue("output")] private readonly string _endId = "rx";
    private readonly List<Module> _modules = [];
    private readonly Module _startModule = new(StartId);
    private Module? _endModule;

    public void Setup(string[] input)
    {
        _endModule = new Module(_endId);
        _modules.Add(_endModule);

        Dictionary<Module, string[]> moduleToOutputs = [];

        foreach (var line in input)
        {
            var split = line.Split(" -> ");
            var source = split[0];
            var outputIds = split[1].Split(", ");
            var id = source[1..];

            var module = source[0] switch
            {
                FlipFlopChar => new FlipFlop(id),
                ConjunctionChar => new Conjunction(id),
                _ => _startModule
            };

            _modules.Add(module);
            moduleToOutputs.Add(module, outputIds);
        }

        // assign outputs
        foreach (var (module, outputIds) in moduleToOutputs)
            module.Outputs = outputIds.Select(id => _modules.First(m => m.Id == id)).ToArray();

        // assign inputs
        foreach (var module in _modules)
            module.Inputs = _modules.Where(m => m.Outputs.Contains(module)).ToArray();
    }

    public object SolvePart1()
    {
        long lowPulses = 1000, highPulses = 0;
        for (var i = 0; i < 1000; i++)
        {
            var (low, high) = PressButton(_startModule);
            (lowPulses, highPulses) = (lowPulses + low, highPulses + high);
        }

        return lowPulses * highPulses;
    }

    public object SolvePart2()
    {
        _modules.ForEach(m => m.Reset());

        // grab the inputs of the next-to-last module as "gatekeepers"
        var gateKeepers = _endModule!.Inputs[0].Inputs;
        var periods = new long[gateKeepers.Length];
        long buttonPresses = 0, i = 0;

        while (i < gateKeepers.Length)
        {
            buttonPresses++;
            if (PressButton(_startModule, gateKeepers)) // if it lights up any of the gatekeepers
                periods[i++] = buttonPresses;
        }

        return periods.Product(); // Least Common Multiple
    }

    private static (long LowPulses, long HighPulses) PressButton(Module start)
    {
        long lowPulses = 0, highPulses = 0;
        Queue<Module> queue = new();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var isHighPulse = current.State;
            var outputs = current.Outputs;

            if (isHighPulse) highPulses += outputs.Length;
            else lowPulses += outputs.Length;

            foreach (var nextModule in outputs)
            {
                nextModule.ProcessPulse(current, isHighPulse);
                if (!isHighPulse || nextModule is Conjunction)
                    queue.Enqueue(nextModule);
            }
        }

        return (lowPulses, highPulses);
    }

    private static bool PressButton(Module start, Module[] gateKeepers)
    {
        var hitNewEndState = false;
        Queue<Module> queue = new();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var isHighPulse = current.State;

            if (isHighPulse && gateKeepers.Contains(current))
                hitNewEndState = true;

            foreach (var nextModule in current.Outputs)
            {
                nextModule.ProcessPulse(current, isHighPulse);
                if (!isHighPulse || nextModule is Conjunction)
                    queue.Enqueue(nextModule);
            }
        }

        return hitNewEndState;
    }

    private class FlipFlop(string id) : Module(id)
    {
        public override void ProcessPulse(Module _, bool pulse) => State ^= !pulse; // toggle on low pulses
    }

    private class Conjunction(string id) : Module(id)
    {
        private bool[] _inputStates = [];

        public override Module[] Inputs
        {
            set
            {
                base.Inputs = value;
                _inputStates = new bool[value.Length];
            }
        }

        public override bool State => !_inputStates.All(p => p);

        public override void ProcessPulse(Module source, bool pulse) =>
            _inputStates[Array.IndexOf(Inputs, source)] = pulse;

        public override void Reset() => _inputStates = new bool[Inputs.Length];
    }

    private class Module(string id)
    {
        public string Id { get; } = id;
        public Module[] Outputs { get; set; } = [];
        public virtual Module[] Inputs { get; set; } = [];
        public virtual bool State { get; protected set; }

        public virtual void ProcessPulse(Module source, bool pulse)
        {
        }

        public virtual void Reset() => State = false;
    }
}