using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2024.D09;

public class Solution : ISolver
{
    private IEnumerable<FileBlock> _files = [];
    private List<int> _gaps = [];

    public void Setup(string[] input)
    {
        var line = input[0];
        _files = Enumerable.Range(0, (line.Length + 1) / 2)
            .Select(i => new FileBlock(i, line[i * 2].AsDigit()));
        _gaps = Enumerable.Range(0, line.Length / 2).Select(i => line[i * 2 + 1].AsDigit()).ToList();
    }

    public object SolvePart1()
    {
        FileBlock[] files = [.._files]; // make a copy

        long checksum = 0;
        long index = files[0].Size; // skip first group of zeroes
        var latestIndex = files.Length - 1;

        for (var i = 0; i < latestIndex; i++)
        {
            var gap = _gaps[i];
            while (gap > 0)
            {
                ref var file = ref files[latestIndex];
                var minSize = Math.Min(gap, file.Size);

                checksum += GetChecksum(file.Id, minSize, index);
                index += minSize;
                gap -= minSize;
                file.Size -= minSize;

                if (file.Size == 0)
                    latestIndex--;
            }

            var (id, size) = files[i + 1];
            checksum += GetChecksum(id, size, index);
            index += size;
        }

        return checksum;
    }

    public object SolvePart2()
    {
        FileBlock[] files = [.._files];

        long checksum = 0;
        long index = files[0].Size;
        var latestIndex = files.Length - 1;

        for (var i = 0; i < latestIndex; i++)
        {
            var gap = _gaps[i];
            while (gap > 0)
            {
                var foundIndex = Array.FindLastIndex(files, latestIndex, latestIndex - i,
                    f => f.Id > 0 && f.Size <= gap);
                if (foundIndex == -1)
                {
                    index += gap;
                    break;
                }

                ref var file = ref files[foundIndex];
                checksum += GetChecksum(file.Id, file.Size, index);
                index += file.Size;
                gap -= file.Size;
                file.Id = 0;

                while (files[latestIndex].Id == 0)
                    latestIndex--;
            }

            var (id, size) = files[i + 1];
            checksum += GetChecksum(id, size, index);
            index += size;
        }

        return checksum;
    }

    private static long GetChecksum(int id, long size, long index) =>
        id * (size * index + size * (size - 1) / 2); // #calculated #math

    private record struct FileBlock(int Id, int Size);
}