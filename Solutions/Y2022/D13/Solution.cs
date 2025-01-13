using System;
using System.Collections.Generic;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2022.D13;

public class Solution : ISolver
{
    private readonly List<Packet> _allPackets = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
            if (!string.IsNullOrEmpty(line))
                _allPackets.Add(ParseLine(line));
    }

    public object SolvePart1()
    {
        var score = 0;
        for (var i = 0; i < _allPackets.Count; i += 2)
            if (ComparePackets(_allPackets[i], _allPackets[i + 1]) == -1)
                score += i / 2 + 1;

        return score;
    }

    public object SolvePart2()
    {
        var divider1 = new Packet(2);
        var divider2 = new Packet(6);
        _allPackets.Add(divider1);
        _allPackets.Add(divider2);

        _allPackets.Sort(ComparePackets);

        var index1 = _allPackets.IndexOf(divider1) + 1;
        var index2 = _allPackets.IndexOf(divider2) + 1;
        return index1 * index2;
    }

    private static Packet ParseLine(string line)
    {
        Packet? currentPacket = null;
        var split = Utils.NumberPattern().Split(line);
        foreach (var chunk in split)
        {
            if (int.TryParse(chunk, out var number))
            {
                currentPacket!.SubPackets.Add(new Packet(number, currentPacket));
                continue;
            }

            foreach (var symbol in chunk)
                switch (symbol)
                {
                    case ',':
                        continue;
                    case '[':
                    {
                        var subPacket = new Packet(currentPacket);
                        currentPacket?.SubPackets.Add(subPacket);
                        currentPacket = subPacket;
                        break;
                    }
                    case ']':
                        currentPacket = currentPacket?.Parent ?? currentPacket;
                        break;
                }
        }

        return currentPacket!;
    }

    // Recursive. -1 = left is correctly to the left of right. +1 = they're swapped. 0 = tied.
    private static int ComparePackets(Packet left, Packet right)
    {
        // Both have a number assigned
        if (left.Number.HasValue && right.Number.HasValue)
            return left.Number.Value == right.Number.Value ? 0 :
                left.Number.Value < right.Number.Value ? -1 : 1;

        // Mixed types (number vs list) - push the value down into another packet layer
        if (left.Number.HasValue ^ right.Number.HasValue)
        {
            var packet = left.Number.HasValue ? left : right;
            var newValueWrapper = new Packet(packet.Number!.Value, packet);
            packet.Number = null;
            packet.SubPackets.Add(newValueWrapper);
        }

        // Both have a list of more packets.
        var maxToCompare = Math.Min(left.SubPackets.Count, right.SubPackets.Count);
        for (var i = 0; i < maxToCompare; i++)
        {
            var result = ComparePackets(left.SubPackets[i], right.SubPackets[i]);
            if (result != 0) return result;
        }

        return left.SubPackets.Count == right.SubPackets.Count ? 0 :
            left.SubPackets.Count < right.SubPackets.Count ? -1 : 1;
    }

    // Every list and number is contained as its own packet. It's packets all the way down
    private class Packet(Packet? parent = null)
    {
        public readonly Packet? Parent = parent;
        public readonly List<Packet> SubPackets = [];
        public int? Number;

        public Packet(int number, Packet? parent = null) : this(parent) => Number = number;
    }
}