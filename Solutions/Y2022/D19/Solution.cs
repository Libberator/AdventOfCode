using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Solutions.Y2022.D19;

public class Solution : ISolver
{
    private static readonly Materials StartingRobots = new(1, 0, 0, 0);
    private static readonly Materials StartingInventory = new(0, 0, 0, 0);

    private static readonly Materials[] Robots =
    [
        StartingRobots, // ore
        new(0, 1, 0, 0), // clay
        new(0, 0, 1, 0), // obsidian
        new(0, 0, 0, 1) // geode
    ];

    private readonly List<Blueprint> _blueprints = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var numbers = Utils.NumberPattern().Matches(line);

            var id = int.Parse(numbers[0].ValueSpan);
            var ore = int.Parse(numbers[1].ValueSpan);
            var clay = int.Parse(numbers[2].ValueSpan);
            var obsOre = int.Parse(numbers[3].ValueSpan);
            var obsClay = int.Parse(numbers[4].ValueSpan);
            var geodeOre = int.Parse(numbers[5].ValueSpan);
            var geodeObs = int.Parse(numbers[6].ValueSpan);

            var blueprint = new Blueprint(id, ore, clay, obsOre, obsClay, geodeOre, geodeObs);
            _blueprints.Add(blueprint);
        }
    }

    public object SolvePart1() =>
        _blueprints.Sum(b => b.Id * FindMaxGeodes(b, 24));

    public object SolvePart2() =>
        _blueprints.Take(3).Select(b => FindMaxGeodes(b, 32)).Product();

    private static int FindMaxGeodes(Blueprint bp, int time)
    {
        var max = 0;
        return FindMaxGeodes(bp, time, StartingRobots, StartingInventory, ref max, []);
    }

    private static int FindMaxGeodes(Blueprint bp, int time, Materials robots, Materials inventory, ref int max,
        HashSet<int> visited)
    {
        var totalGeodes = inventory.Geode + time * robots.Geode;
        if (time <= 1) return totalGeodes;

        // Pruning. Removing branches where even the most optimistic (buying geode robot every round) won't beat current max
        if (totalGeodes + Utils.TriangleSum(time - 1) <= max) return max;

        var snapshot = HashCode.Combine(robots, inventory);
        if (!visited.Add(snapshot)) return max;

        // go to next nodes
        foreach (var (robotPurchased, robotCost, timeSpent) in NextNodes(bp, robots, inventory, time))
        {
            var nextRobots = robots + robotPurchased;
            var nextInventory = inventory - robotCost + timeSpent * robots;
            max = Math.Max(max, FindMaxGeodes(bp, time - timeSpent, nextRobots, nextInventory, ref max, visited));
        }

        return Math.Max(max, totalGeodes);
    }

    private static IEnumerable<(Materials RobotPurchased, Materials RobotCost, int MinutesSpent)> NextNodes(
        Blueprint blueprint, Materials robots, Materials inventory, int minutes)
    {
        for (var i = 3; i >= 0; i--)
        {
            var cost = blueprint.RobotCost(i);
            if (!TryPurchaseRobot(cost, robots, inventory, out var reqMinutes) || minutes <= reqMinutes)
                continue;
            yield return (Robots[i], cost, reqMinutes);
        }
    }

    private static bool TryPurchaseRobot(Materials cost, Materials robots, Materials inventory, out int reqMinutes)
    {
        reqMinutes = 1;
        if (inventory >= cost) return true;

        for (var i = 0; i < 4; i++)
        {
            if (cost[i] <= 0) continue;
            if (robots[i] == 0) return false; // no means of producing required material to buy this robot
            var minutesToEarnMaterial = 1 + (int)Math.Ceiling((cost[i] - inventory[i]) / (double)robots[i]);
            reqMinutes = Math.Max(reqMinutes, minutesToEarnMaterial);
        }

        return true;
    }

    private class Blueprint(int id, int ore, int clay, int obsOre, int obsClay, int geodeOre, int geodeObs)
    {
        private readonly Materials _clayRobotCost = new(clay, 0, 0, 0);
        private readonly Materials _geodeRobotCost = new(geodeOre, 0, geodeObs, 0);
        private readonly Materials _obsidianRobotCost = new(obsOre, obsClay, 0, 0);
        private readonly Materials _oreRobotCost = new(ore, 0, 0, 0);
        public readonly int Id = id;

        public Materials RobotCost(int index) => index switch
        {
            0 => _oreRobotCost,
            1 => _clayRobotCost,
            2 => _obsidianRobotCost,
            3 => _geodeRobotCost,
            _ => throw new IndexOutOfRangeException($"{index} is out of range")
        };
    }

    private readonly record struct Materials(int Ore, int Clay, int Obsidian, int Geode)
    {
        public int this[int index] => index switch
        {
            0 => Ore,
            1 => Clay,
            2 => Obsidian,
            3 => Geode,
            _ => throw new IndexOutOfRangeException($"{index} is out of range")
        };

        public static Materials operator +(Materials a, Materials b) => new(a.Ore + b.Ore, a.Clay + b.Clay,
            a.Obsidian + b.Obsidian, a.Geode + b.Geode);

        public static Materials operator -(Materials a, Materials b) => new(a.Ore - b.Ore, a.Clay - b.Clay,
            a.Obsidian - b.Obsidian, a.Geode - b.Geode);

        public static Materials operator *(int a, Materials b) => new(a * b.Ore, a * b.Clay, a * b.Obsidian, a * b.Geode);

        public static bool operator >(Materials a, Materials b) =>
            a.Ore > b.Ore && a.Clay > b.Clay && a.Obsidian > b.Obsidian && a.Geode > b.Geode;

        public static bool operator <(Materials a, Materials b) =>
            a.Ore < b.Ore && a.Clay < b.Clay && a.Obsidian < b.Obsidian && a.Geode < b.Geode;

        public static bool operator >=(Materials a, Materials b) =>
            a.Ore >= b.Ore && a.Clay >= b.Clay && a.Obsidian >= b.Obsidian && a.Geode >= b.Geode;

        public static bool operator <=(Materials a, Materials b) =>
            a.Ore <= b.Ore && a.Clay <= b.Clay && a.Obsidian <= b.Obsidian && a.Geode <= b.Geode;
    }
}