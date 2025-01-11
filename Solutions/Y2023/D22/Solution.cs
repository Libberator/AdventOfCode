using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2023.D22;

public class Solution : ISolver
{
    private readonly List<Brick> _bricks = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split('~');
            var lower = Vec3D.Parse(split[0]);
            var upper = Vec3D.Parse(split[1]);
            _bricks.Add(new Brick(lower, upper));
        }

        ApplyGravity(_bricks);
    }

    public object SolvePart1() => _bricks.Count(b => b.CanBeDisintegrated());

    public object SolvePart2() => _bricks.Sum(b => b.SupportingTotal());

    private static void ApplyGravity(List<Brick> bricks)
    {
        bricks.Sort((a, b) => a.Bottom.CompareTo(b.Bottom));

        for (var i = 0; i < bricks.Count; i++)
        {
            var brick = bricks[i];
            var bottom = 1;

            var bricksBelow = bricks.Take(i).Where(b => b.OverlapsXy(brick)).ToArray();
            if (bricksBelow.Length != 0)
            {
                var highestZ = bricksBelow.Max(b => b.Top);
                bottom = highestZ + 1;
                foreach (var brickBelow in bricksBelow.Where(b => b.Top == highestZ))
                {
                    brick.Below.Add(brickBelow);
                    brickBelow.Above.Add(brick);
                }
            }

            brick.MoveDownBy(brick.Bottom - bottom);
        }
    }

    private class Brick(Vec3D lower, Vec3D upper)
    {
        public readonly List<Brick> Above = [];
        public readonly List<Brick> Below = [];
        private Vec3D _lower = lower;
        private Vec3D _upper = upper;
        public int Bottom => _lower.Z;
        public int Top => _upper.Z;

        public bool OverlapsXy(Brick other) =>
            _lower.X <= other._upper.X && other._lower.X <= _upper.X &&
            _lower.Y <= other._upper.Y && other._lower.Y <= _upper.Y;

        public void MoveDownBy(int zDistance)
        {
            _lower -= new Vec3D(0, 0, zDistance);
            _upper -= new Vec3D(0, 0, zDistance);
        }

        public bool CanBeDisintegrated() => Above.All(a => a.Below.Count > 1);

        public int SupportingTotal() => SupportedBy(this, []);

        private static int SupportedBy(Brick disintegrated, HashSet<Brick> supported)
        {
            supported.Add(disintegrated);
            List<Brick> toFallNext = [];

            foreach (var above in disintegrated.Above)
            {
                if (!supported.IsSupersetOf(above.Below)) continue;
                toFallNext.Add(above);
            }

            return toFallNext.Count + toFallNext.Sum(next => SupportedBy(next, supported));
        }
    }
}