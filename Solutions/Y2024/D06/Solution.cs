using System.Collections.Generic;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2024.D06;

public class Solution : ISolver
{
    private readonly HashSet<Vec2D> _obstacles = [];
    private readonly Dictionary<Vec2D, Vec2D> _visited = [];
    private Vec2D _gridSize;
    private Vec2D _startPos;

    public void Setup(string[] input)
    {
        _gridSize = input.GetGridSize();

        foreach (var pos in _gridSize.GeneratePoints())
            switch (input.GetAt(pos))
            {
                case '#': _obstacles.Add(pos); break;
                case '^': _startPos = pos; break;
            }
    }

    public object SolvePart1()
    {
        var pose = new Pose2D(_startPos, Vec2D.N);
        _visited.Add(pose.Pos, pose.Dir);

        while (pose.Ahead.IsWithinBounds(_gridSize))
        {
            pose = _obstacles.Contains(pose.Ahead) ? pose.TurnRight() : pose.Step();
            _visited.TryAdd(pose.Pos, pose.Dir);
        }

        return _visited.Count;
    }

    public object SolvePart2()
    {
        var newObstacles = 0;

        foreach (var (pos, dir) in _visited)
        {
            if (pos == _startPos) continue;
            if (!WillLoop(pos - dir, dir, pos)) continue;
            newObstacles++;
        }

        return newObstacles;
    }

    private bool WillLoop(Vec2D pos, Vec2D dir, Vec2D obstaclePos)
    {
        HashSet<(Vec2D, Vec2D)> visited = [];
        _obstacles.Add(obstaclePos);
        dir = dir.RotatedRight();

        while (visited.Add((pos, dir)))
        {
            while (!_obstacles.Contains(pos += dir))
            {
                if (pos.IsWithinBounds(_gridSize)) continue;
                _obstacles.Remove(obstaclePos);
                return false;
            }

            pos -= dir;
            dir = dir.RotatedRight();
        }

        _obstacles.Remove(obstaclePos);
        return true;
    }
}