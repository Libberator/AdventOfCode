using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AoC.Utilities.Geometry;

namespace AoC.Utilities.Extensions;

public static partial class Utils
{
    public static float[] ParseFloats(this string s) => s.ParseMany<float>(FloatPattern()).ToArray();
    public static float[] ParseFloats(this string[] s) => s.ParseMany<float>(FloatPattern()).ToArray();
    public static int[] ParseInts(this string s) => s.ParseMany<int>(NumberPattern()).ToArray();
    public static int[] ParseInts(this string[] s) => s.ParseMany<int>(NumberPattern()).ToArray();
    public static long[] ParseLongs(this string s) => s.ParseMany<long>(NumberPattern()).ToArray();
    public static long[] ParseLongs(this string[] s) => s.ParseMany<long>(NumberPattern()).ToArray();
    public static Vec2D[] ParseVec2Ds(this string s) => s.ParseMany<Vec2D>(Vec2DPattern()).ToArray();
    public static Vec2D[] ParseVec2Ds(this string[] s) => s.ParseMany<Vec2D>(Vec2DPattern()).ToArray();

    /// <summary>Converts a char to an integer (0-9). This assumes your char matches [0-9].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AsDigit(this char c) => c - '0'; // `c & 0xF;` also works (same performance) but is less readable

    /// <summary>Converts a char to an integer (0-25). This assumes your char matches [A-Za-z].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AsIndex(this char c) => (c - 1) & 0x1F; // `char.ToLower(c) - 'a';` also works but is slower

    /// <summary>Chunk the source between null or whitespace strings.</summary>
    public static IList<string[]> ChunkByNonEmpty(this IEnumerable<string> source) =>
        source.ChunkBy(s => !string.IsNullOrWhiteSpace(s));

    /// <summary>
    ///     Get the size of the grid of characters as (rows, cols). This assumes it's rectangular and has at least 1 row.
    /// </summary>
    public static Vec2D GetGridSize(this string[] grid) => new(grid.Length, grid[0].Length);

    /// <summary>Finds first instance of character, returns position as (row, col). If not found, returns (-1,-1).</summary>
    public static Vec2D FindPosOf(this string[] data, char target)
    {
        var y = -1;
        var x = Array.FindIndex(data, line => (y = line.IndexOf(target)) != -1);
        return new Vec2D(x, y);
    }

    /// <summary>Tries to find first instance of character, outputs position as (row, col).</summary>
    public static bool TryFindPosOf(this string[] data, char target, out Vec2D pos)
    {
        pos = data.FindPosOf(target);
        return pos.X != -1 && pos.Y != -1;
    }

    /// <summary>Note: the X value of <paramref name="pos" /> is the string selector, the Y value is the char index.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char GetAt(this string[] data, Vec2D pos) => data[pos.X][pos.Y];

    /// <summary>
    ///     Replaces the string with the character at <paramref name="pos" /> changed to <paramref name="value" />.
    /// </summary>
    public static void SetAt(this string[] data, Vec2D pos, char value) =>
        data[pos.X] = data[pos.X].SetAt(pos.Y, value);

    /// <summary>
    ///     Returns a new string with the character at <paramref name="index" /> changed to <paramref name="value" />.
    /// </summary>
    public static string SetAt(this string s, int index, char value)
    {
        var span = s.AsSpan();
        Span<char> buffer = stackalloc char[span.Length];
        span.CopyTo(buffer);
        buffer[index] = value;
        return new string(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string RemoveWhiteSpace(this string str) => WhiteSpacePattern().Replace(str, string.Empty);

    /// <summary>Returns a concatenated string with the source <paramref name="s" /> repeated <paramref name="n" /> times.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Repeat(this string s, int n) => new StringBuilder(n * s.Length).Insert(0, s, n).ToString();

    /// <summary>Returns a concatenated string with the source <paramref name="c" /> repeated <paramref name="n" /> times.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Repeat(this char c, int n) => new(c, n);

    /// <summary>Returns a new string[] with the source data transposed (col swapped with row). Does not modify in-place.</summary>
    public static string[] Transpose(this string[] source)
    {
        var cols = source[0].Length;
        if (source.Any(line => line.Length != cols))
            throw new ArgumentException($"Invalid data length. Not all lines are {cols} characters long");

        var result = new string[cols];
        var sb = new StringBuilder();
        for (var c = 0; c < cols; c++)
        {
            foreach (var line in source)
                sb.Append(line[c]);
            result[c] = sb.ToString();
            sb.Clear();
        }

        return result;
    }
}