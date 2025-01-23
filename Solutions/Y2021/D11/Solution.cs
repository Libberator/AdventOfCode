using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC.Solutions.Y2021.D11;

public class Solution : ISolver
{
    private readonly List<Octopus> _activeOctopuses = [];
    private readonly int[,] _data = new int[10, 10];
    private int _numberOfFlashes;

    public void Setup(string[] input)
    {
        var row = 0;
        foreach (var line in input)
        {
            var col = 0;
            foreach (var letter in line)
            {
                _data[row, col] = letter - '0';
                col++;
            }

            row++;
        }
    }

    public object SolvePart1()
    {
        ResetOctopuses();
        _numberOfFlashes = 0;
        for (var i = 0; i < 100; i++)
        {
            StepEvent?.Invoke();
            CheckEvent?.Invoke();
        }

        return _numberOfFlashes;
    }

    public object SolvePart2()
    {
        ResetOctopuses();
        var currentStep = 0;
        while (!_activeOctopuses.All(o => o.HasFlashed))
        {
            currentStep++;
            StepEvent?.Invoke();
            CheckEvent?.Invoke();
        }

        return currentStep;
    }

    public event Action StepEvent = delegate { };
    public event Action CheckEvent = delegate { };
    public event Action<int, int> FlashEvent = delegate { };

    private void TriggerFlash(int row, int col)
    {
        FlashEvent.Invoke(row, col);
        _numberOfFlashes++;
    }

    private void ResetOctopuses()
    {
        StepEvent = delegate { };
        CheckEvent = delegate { };
        FlashEvent = delegate { };
        _activeOctopuses.Clear();
        for (var i = 0; i < 10; i++)
            for (var j = 0; j < 10; j++)
                _activeOctopuses.Add(new Octopus(this, _data[i, j], i, j));
    }

    private class Octopus
    {
        private readonly int _row, _col;
        private readonly Solution _source;
        private int _value;

        public Octopus(Solution source, int value, int row, int col)
        {
            _source = source;
            _value = value;
            _row = row;
            _col = col;
            source.FlashEvent += NeighborFlashed;
            source.StepEvent += Step;
            source.CheckEvent += Check;
        }

        public bool HasFlashed { get; private set; }

        private void Step()
        {
            _value = _value > 9 ? 1 : _value + 1;
            HasFlashed = false;
        }

        private void NeighborFlashed(int row, int col)
        {
            if (!IsWithinRange(row, col)) return;
            _value++;
            Check();
        }

        private void Check()
        {
            if (HasFlashed) return;
            if (_value > 9) Flash();
        }

        private void Flash()
        {
            HasFlashed = true;
            _source.TriggerFlash(_row, _col);
        }

        private bool IsWithinRange(int row, int col) =>
            Math.Abs(row - _row) <= 1 && Math.Abs(col - _col) <= 1 &&
            !(row == _row && col == _col); // not self
    }
}