using System;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2021.D17;

public class Solution : ISolver
{
    private Vec2D _minBounds, _maxBounds;
    private int _minXVel, _maxXVel, _minYVel, _maxYVel;

    public void Setup(string[] input)
    {
        var matches = Utils.NumberPattern().Matches(input[0]);
        _minBounds = new Vec2D(int.Parse(matches[0].Value), int.Parse(matches[2].Value));
        _maxBounds = new Vec2D(int.Parse(matches[1].Value), int.Parse(matches[3].Value));

        // X position is determined via Triangle Sum (n * (n + 1) / 2). Solve quadratic for positive root, rounded up
        _minXVel = (int)Math.Ceiling((Math.Sqrt(8 * _minBounds.X + 1) - 1) / 2);
        _maxXVel = (_maxBounds.X + 1) / 2;
        _minYVel = (_minBounds.Y + 1) / 2;
        _maxYVel = Math.Abs(_minBounds.Y) - 1; // what goes up must come down. Maximize depth and avoid overshooting
    }

    public object SolvePart1() => Utils.TriangleSum(_maxYVel);

    public object SolvePart2() => GetTotalTrajectories();

    private int GetTotalTrajectories()
    {
        var successes = (_maxBounds.X - _minBounds.X + 1) * (_maxBounds.Y - _minBounds.Y + 1);
        for (var xVel = _minXVel; xVel <= _maxXVel; xVel++)
            for (var yVel = _minYVel; yVel <= _maxYVel; yVel++)
                if (TrajectorySuccess(new Vec2D(xVel, yVel)))
                    successes++;

        return successes;
    }

    private bool TrajectorySuccess(Vec2D vel)
    {
        var pos = Vec2D.Zero;
        while (!HasOvershot(pos))
        {
            pos += vel;
            vel -= vel.X > 0 ? new Vec2D(1, 0) : Vec2D.Zero; // drag
            vel -= new Vec2D(0, 1); // gravity
            if (pos.IsWithinBoundsInclusive(_minBounds, _maxBounds))
                return true;
        }

        return false;

        bool HasOvershot(Vec2D p) => p.X > _maxBounds.X || p.Y < _minBounds.Y;
    }
}