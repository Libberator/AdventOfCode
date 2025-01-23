using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2021.D16;

public class Solution : ISolver
{
    private Packet? _rootPacket;

    public void Setup(string[] input)
    {
        var sb = new StringBuilder();
        foreach (var hex in input[0])
            sb.Append(hex.HexToBinary());

        var index = 0;
        _rootPacket = ParsePackets(sb.ToString(), ref index);
    }

    public object SolvePart1() => SumVersionNumbers(_rootPacket!);

    public object SolvePart2() => _rootPacket!.GetValue();

    private static int SumVersionNumbers(Packet root)
    {
        var sum = 0;
        var stack = new Stack<Packet>([root]);

        while (stack.Count > 0)
        {
            var packet = stack.Pop();
            sum += packet.Version;
            foreach (var subPacket in packet.SubPackets)
                stack.Push(subPacket);
        }

        return sum;
    }

    private static Packet ParsePackets(string data, ref int index)
    {
        var version = data[index..(index + 3)].ToIntFromBase(2);
        index += 3;
        var typeId = data[index..(index + 3)];
        index += 3;

        var packet = new Packet(version, typeId);

        if (typeId == "100")
        {
            var reachedStopBit = false;
            StringBuilder sb = new();
            while (!reachedStopBit)
            {
                var chunk = data[index..(index + 5)];
                index += 5;
                reachedStopBit = chunk[0] == '0';
                sb.Append(chunk.AsSpan(1));
            }

            packet.LiteralValue = sb.ToString().ToLongFromBase(2);
            return packet;
        }

        var lengthTypeId = data[index++];
        var nextBitQty = lengthTypeId == '0' ? 15 : 11;
        var parseQty = data[index..(index + nextBitQty)].ToIntFromBase(2);
        index += nextBitQty;

        switch (typeId)
        {
            case "000" or "001" or "010" or "011":
                if (lengthTypeId == '0')
                {
                    var targetIndex = index + parseQty;
                    while (index < targetIndex)
                        packet.SubPackets.Add(ParsePackets(data, ref index));
                }
                else
                {
                    for (var i = 0; i < parseQty; i++)
                        packet.SubPackets.Add(ParsePackets(data, ref index));
                }

                break;
            case "101" or "110" or "111":
                packet.SubPackets.Add(ParsePackets(data, ref index));
                packet.SubPackets.Add(ParsePackets(data, ref index));
                break;
        }

        return packet;
    }

    private class Packet(int version, string typeId)
    {
        public readonly List<Packet> SubPackets = [];
        public readonly string TypeId = typeId;
        public readonly int Version = version;
        public long LiteralValue;

        public long GetValue() => TypeId switch
        {
            "000" => SubPackets.Sum(s => s.GetValue()),
            "001" => SubPackets.Select(s => s.GetValue()).Product(),
            "010" => SubPackets.Min(s => s.GetValue()),
            "011" => SubPackets.Max(s => s.GetValue()),
            "100" => LiteralValue,
            "101" => SubPackets[0].GetValue() > SubPackets[1].GetValue() ? 1 : 0,
            "110" => SubPackets[0].GetValue() < SubPackets[1].GetValue() ? 1 : 0,
            "111" => SubPackets[0].GetValue() == SubPackets[1].GetValue() ? 1 : 0,
            _ => throw new ArgumentOutOfRangeException(nameof(TypeId))
        };
    }
}