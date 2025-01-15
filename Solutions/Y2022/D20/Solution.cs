using System.Collections.Generic;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2022.D20;

public class Solution : ISolver
{
    private const int DecryptionKey = 811_589_153;
    private static int _count;
    private readonly List<Ref<int>> _data = [];

    public void Setup(string[] input)
    {
        foreach (var line in input) _data.Add(new Ref<int>(int.Parse(line)));
        _count = _data.Count;
    }

    public object SolvePart1() => GetGroveCoordinates(MixByValue([.._data]));

    public object SolvePart2()
    {
        var data = new List<Ref<int>>(_data);
        var multiplier = DecryptionKey % (_count - 1);
        for (var i = 0; i < 10; i++)
            _ = MixByValue(data, multiplier);
        return GetGroveCoordinates(data, DecryptionKey);
    }

    private List<Ref<int>> MixByValue(List<Ref<int>> movingData, int multiplier = 1)
    {
        foreach (var item in _data)
        {
            var value = item.Value * multiplier;
            var index = movingData.IndexOf(item);
            var nextIndex = (index + value).Mod(_count - 1);
            movingData.Remove(item);
            movingData.Insert(nextIndex, item);
        }

        return movingData;
    }

    private static long GetGroveCoordinates(List<Ref<int>> data, long multiplier = 1)
    {
        var zeroIndex = data.FindIndex(d => d.Value == 0);
        var a = data[(zeroIndex + 1000) % _count].Value;
        var b = data[(zeroIndex + 2000) % _count].Value;
        var c = data[(zeroIndex + 3000) % _count].Value;
        return multiplier * (a + b + c);
    }

    // This is so that we can have ints as a reference type for IndexOf lookups while avoiding duplicates
    private class Ref<T>(T value) where T : struct
    {
        public readonly T Value = value;
    }
}