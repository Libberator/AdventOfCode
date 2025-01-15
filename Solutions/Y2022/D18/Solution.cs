using System.Collections.Generic;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D18;

public class Solution : ISolver
{
    private readonly HashSet<Vec3D> _cubes = [];
    private Vec3D _lowerCorner, _upperCorner;

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var pos = Vec3D.Parse(line);
            _cubes.Add(pos);
            _lowerCorner = Vec3D.Min(_lowerCorner, pos);
            _upperCorner = Vec3D.Max(_upperCorner, pos);
        }
    }

    public object SolvePart1()
    {
        var totalFaceCount = 6 * _cubes.Count;
        var facesHidden = 0;
        foreach (var pos in _cubes)
            foreach (var dir in Vec3D.PrimaryDirs)
                if (_cubes.Contains(pos + dir))
                    facesHidden++;
        return totalFaceCount - facesHidden;
    }

    public object SolvePart2() =>
        GetSurfaceArea(_lowerCorner - Vec3D.One, _upperCorner + Vec3D.One);

    private int GetSurfaceArea(Vec3D lowerCorner, Vec3D upperCorner)
    {
        var surfaceArea = 0;
        Queue<Vec3D> queue = [];
        HashSet<Vec3D> visited = [];
        queue.Enqueue(lowerCorner);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!visited.Add(current)) continue;

            foreach (var dir in Vec3D.PrimaryDirs)
            {
                var pos = current + dir;
                if (!pos.IsWithinBoundsInclusive(lowerCorner, upperCorner)) continue;
                if (_cubes.Contains(pos)) surfaceArea++;
                else queue.Enqueue(pos);
            }
        }

        return surfaceArea;
    }
}