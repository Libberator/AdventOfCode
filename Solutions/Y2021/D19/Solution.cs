using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2021.D19;

public class Solution : ISolver
{
    private readonly HashSet<Vec3D> _beacons = [];
    private readonly Quaternion[] _rotations = new Quaternion[24];
    private readonly List<Vec3D> _scanners = [];

    public void Setup(string[] input)
    {
        var scans = input.ChunkByNonEmpty().Select(chunk => Array.ConvertAll(chunk[1..], Vec3D.Parse)).ToList();
        InitializeRotations(_rotations);
        MapBeaconsAndScanners(_scanners, _beacons, scans);
    }

    public object SolvePart1() => _beacons.Count;

    public object SolvePart2()
    {
        var max = 0;
        for (var i = 0; i < _scanners.Count - 1; i++)
            for (var j = i + 1; j < _scanners.Count; j++)
                max = Math.Max(max, _scanners[i].DistanceManhattan(_scanners[j]));
        return max;
    }

    private void MapBeaconsAndScanners(List<Vec3D> scanners, HashSet<Vec3D> beacons, List<Vec3D[]> scans)
    {
        scanners.Add(Vec3D.Zero);
        foreach (var beacon in scans[0])
            beacons.Add(beacon);
        scans.RemoveAt(0);

        while (scans.Count > 0)
            for (var i = scans.Count - 1; i >= 0; i--)
            {
                if (!TryGetMapping(beacons, scans[i], out var offset, out var rotatedPoints))
                    continue;

                scanners.Add(offset);
                foreach (var rotated in rotatedPoints)
                    beacons.Add(rotated + offset);
                scans.RemoveAt(i);
            }
    }

    private bool TryGetMapping(HashSet<Vec3D> knownPoints, Vec3D[] points, out Vec3D offset, out Vec3D[] rotatedPoints)
    {
        foreach (var rot in _rotations)
        {
            rotatedPoints = points.Select(p => rot * p).ToArray();
            if (TryFindOffset(knownPoints, rotatedPoints, out offset))
                return true;
        }

        offset = Vec3D.Zero;
        rotatedPoints = [];
        return false;
    }

    private static bool TryFindOffset(HashSet<Vec3D> knownPoints, Vec3D[] rotatedPoints, out Vec3D offset)
    {
        foreach (var knownPos in knownPoints)
            for (var i = 0; i < 5; i++) // reduce search space via pigeonhole principle: rotatedPoints.Length - 11
            {
                offset = knownPos - rotatedPoints[i];

                var matched = 0;
                for (var remaining = rotatedPoints.Length - 1; remaining >= 0; remaining--)
                {
                    if (knownPoints.Contains(rotatedPoints[remaining] + offset) && ++matched >= 12)
                        return true;

                    if (matched + remaining < 12)
                        break; // if we can't attain 12 minimum, bail out
                }
            }

        offset = Vec3D.Zero;
        return false;
    }

    private static void InitializeRotations(Quaternion[] rotations)
    {
        (Quaternion, Axis)[] transformations =
        [
            (Quaternion.Identity, Axis.X), // +x
            (Quaternion.P180Y, Axis.X), // -x
            (Quaternion.P90Z, Axis.Y), // +y
            (Quaternion.N90Z, Axis.Y), // -y
            (Quaternion.P90Y, Axis.Z), // +z
            (Quaternion.N90Y, Axis.Z) // -z
        ];

        var i = 0;
        foreach (var (orientation, axis) in transformations)
            foreach (var angle in Angle.Cardinals)
                rotations[i++] = orientation * Quaternion.FromAxisAngle(axis, angle);
    }
}