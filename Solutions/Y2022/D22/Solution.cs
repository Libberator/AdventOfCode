using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;
using AoC.Utilities.Geometry;

namespace AoC.Solutions.Y2022.D22;

public class Solution : ISolver
{
    private const string Left = "L", Right = "R";
    private const char Space = ' ', Walkable = '.', Wall = '#';

    private readonly Dictionary<Vec2D, List<Edge>> _edgeConnections = new()
    {
        { Vec2D.N, [] }, { Vec2D.E, [] }, { Vec2D.S, [] }, { Vec2D.W, [] }
    };

    private string[] _instructions = [];
    private string[] _grid = [];
    private Vec2D _startPosition;

    public void Setup(string[] input)
    {
        _grid = input[..^2];
        _instructions = Utils.NumberPattern().Split(input[^1])[1..];
        _startPosition = new Vec2D(0, _grid[0].IndexOf(Walkable));
    }

    public object SolvePart1()
    {
        var pos = _startPosition;
        var dir = Vec2D.E;

        foreach (var instruction in _instructions)
            switch (instruction)
            {
                case Left: dir = dir.RotatedLeft(); break;
                case Right: dir = dir.RotatedRight(); break;
                default:
                {
                    if (int.TryParse(instruction, out var steps)) TakeSteps(ref pos, dir, steps);
                    break;
                }
            }
        var pw = GetPassword(pos, dir);
        return pw;
    }

    public object SolvePart2()
    {
        InitEdgeConnections();
        var pos = _startPosition;
        var dir = Vec2D.E;

        foreach (var instruction in _instructions)
            switch (instruction)
            {
                case Left: dir = dir.RotatedLeft(); break;
                case Right: dir = dir.RotatedRight(); break;
                default:
                {
                    if (int.TryParse(instruction, out var steps)) TakeSteps3D(ref pos, ref dir, steps);
                    break;
                }
            }

        var pw = GetPassword(pos, dir);
        return pw;
    }

    private void TakeSteps(ref Vec2D pos, Vec2D dir, int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var nextPos = NextPosition(pos, dir);
            if (_grid[nextPos.X][nextPos.Y] == Wall) break;
            pos = nextPos;
        }

        return;

        Vec2D NextPosition(Vec2D pos, Vec2D direction)
        {
            pos += direction;
            while (IsOutOfBounds(pos))
                if (pos.X < 0) pos = pos with { X = _grid.Length - 1 }; // loop N to S
                else if (pos.X >= _grid.Length) pos = pos with { X = 0 }; // loop S to N
                else if (pos.Y < 0) pos = pos with { Y = _grid[pos.X].Length - 1 }; // loop W to E
                else if (pos.Y >= _grid[pos.X].Length && direction == Vec2D.E) pos = pos with { Y = 0 }; // loop E to W
                else pos += direction;
            return pos;
        }
    }

    private void TakeSteps3D(ref Vec2D pos, ref Vec2D dir, int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var (nextPos, nextDir) = NextMove(pos, dir);
            if (_grid[nextPos.X][nextPos.Y] == Wall) break;
            pos = nextPos;
            dir = nextDir;
        }

        return;

        (Vec2D NextPos, Vec2D NextDir) NextMove(Vec2D pos, Vec2D dir)
        {
            var nextPos = pos + dir;
            if (!IsOutOfBounds(nextPos)) return (nextPos, dir);
            foreach (var edge in _edgeConnections[dir])
                if (edge.Contains(pos))
                {
                    nextPos = edge.GetNextPosition(pos);
                    dir = edge.NextDir;
                    break;
                }

            return (nextPos, dir);
        }
    }

    private bool IsOutOfBounds(Vec2D pos)
    {
        if (pos.X < 0 || pos.X >= _grid.Length) return true;
        if (pos.Y < 0 || pos.Y >= _grid[pos.X].Length) return true;
        return _grid[pos.X][pos.Y] == Space;
    }

    private static int GetPassword(Vec2D pos, Vec2D dir) => 1000 * (pos.X + 1) + 4 * (pos.Y + 1) + DirectionValue(dir);

    private static int DirectionValue(Vec2D dir) => dir switch
    {
        { X: -1, Y: 0 } => 3, // NORTH
        { X: 0, Y: -1 } => 2, // WEST
        { X: 1, Y: 0 } => 1, // SOUTH
        { X: 0, Y: 1 } => 0, // EAST
        _ => -1
    };

    #region Connecting Edges For a Cube Net

    private void InitEdgeConnections()
    {
        var sideLength = Utils.GreatestCommonDivisor(_grid.Length, _grid.Max(line => line.Length));
        var startingFacePos = _startPosition / sideLength;

        // Corner IDs. These get rotated around as the cube rolls to simulate unfolding
        int topLeft = 0,
            topRight = 1,
            botLeft = 2,
            botRight = 3; // the main 4 IDs that will be "stamped" to each face to line up corners/edges
        int topLeftFront = 4,
            topRightFront = 5,
            botLeftFront = 6,
            botRightFront = 7; // the other IDs directly above (or in front) to form the cube

        HashSet<Vec2D> visited = [];
        Dictionary<int, (Vec2D Start, Vec2D End, Vec2D ExitDirection)> unmatchedPairs = new();

        Recurse(startingFacePos);
        return;

        void Recurse(Vec2D pos, Vec2D dir = default)
        {
            visited.Add(pos);
            RollCube(dir);

            var topLeftPos = sideLength * pos;
            var topRightPos = topLeftPos + new Vec2D(0, sideLength - 1);
            var botLeftPos = topLeftPos + new Vec2D(sideLength - 1, 0);
            var botRightPos = botLeftPos + new Vec2D(0, sideLength - 1);

            MatchEdges(topLeftPos, topRightPos, topLeft, topRight, Vec2D.N);
            MatchEdges(topRightPos, botRightPos, topRight, botRight, Vec2D.E);
            MatchEdges(botLeftPos, botRightPos, botLeft, botRight, Vec2D.S);
            MatchEdges(topLeftPos, botLeftPos, topLeft, botLeft, Vec2D.W);

            foreach (var direction in Vec2D.CardinalDirs)
            {
                var nextPos = pos + direction;
                if (IsOutOfBounds(sideLength * nextPos)) continue;
                if (visited.Contains(nextPos)) continue;
                Recurse(nextPos, direction);
            }

            RollCube(dir, true);
        }

        void MatchEdges(Vec2D start, Vec2D end, int startCornerId, int endCornerId, Vec2D direction)
        {
            if (startCornerId > endCornerId)
                (start, end) = (end, start); //to ensure we match the right corner pairs together
            var edgeId = (1 << startCornerId) | (1 << endCornerId); // unique value that only 2 faces will connect to

            if (unmatchedPairs.TryGetValue(edgeId, out var pair))
            {
                _edgeConnections[direction].Add(new Edge(start, end, pair.Start, pair.End, -pair.ExitDirection));
                _edgeConnections[pair.ExitDirection].Add(new Edge(pair.Start, pair.End, start, end, -direction));
            }
            else
            {
                unmatchedPairs.Add(edgeId, (start, end, direction));
            }
        }

        void RollCube(Vec2D dir, bool undo = false)
        {
            switch (dir)
            {
                case { X: -1, Y: 0 }:
                    if (undo) RollDown();
                    else RollUp();
                    break; // North
                case { X: 0, Y: 1 }:
                    if (undo) RollLeft();
                    else RollRight();
                    break; // East
                case { X: 1, Y: 0 }:
                    if (undo) RollUp();
                    else RollDown();
                    break; // South
                case { X: 0, Y: -1 }:
                    if (undo) RollRight();
                    else RollLeft();
                    break; // West
            }
        }

        void RollRight()
        {
            (topLeft, topRight, topRightFront, topLeftFront) = (topRight, topRightFront, topLeftFront, topLeft);
            (botLeft, botRight, botRightFront, botLeftFront) = (botRight, botRightFront, botLeftFront, botLeft);
        }

        void RollLeft()
        {
            (topLeft, topRight, topRightFront, topLeftFront) = (topLeftFront, topLeft, topRight, topRightFront);
            (botLeft, botRight, botRightFront, botLeftFront) = (botLeftFront, botLeft, botRight, botRightFront);
        }

        void RollDown()
        {
            (topLeft, botLeft, botLeftFront, topLeftFront) = (botLeft, botLeftFront, topLeftFront, topLeft);
            (topRight, botRight, botRightFront, topRightFront) = (botRight, botRightFront, topRightFront, topRight);
        }

        void RollUp()
        {
            (topLeft, botLeft, botLeftFront, topLeftFront) = (topLeftFront, topLeft, botLeft, botLeftFront);
            (topRight, botRight, botRightFront, topRightFront) = (topRightFront, topRight, botRight, botRightFront);
        }
    }

    private record Edge(Vec2D FromStart, Vec2D FromEnd, Vec2D ToStart, Vec2D ToEnd, Vec2D NextDir)
    {
        public bool Contains(Vec2D pos)
        {
            if (FromStart.X == FromEnd.X)
                return pos.X == FromStart.X && pos.Y >= Math.Min(FromStart.Y, FromEnd.Y) &&
                       pos.Y <= Math.Max(FromStart.Y, FromEnd.Y);
            if (FromStart.Y == FromEnd.Y)
                return pos.Y == FromStart.Y && pos.X >= Math.Min(FromStart.X, FromEnd.X) &&
                       pos.X <= Math.Max(FromStart.X, FromEnd.X);
            return false;
        }

        public Vec2D GetNextPosition(Vec2D pos) => Vec2D.Map(pos, FromStart, FromEnd, ToStart, ToEnd);
    }

    #endregion Connecting Edges For a Cube Net
}