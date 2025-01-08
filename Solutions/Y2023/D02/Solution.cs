using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC.Solutions.Y2023.D02;

public class Solution : ISolver
{
    private const string Red = "red", Green = "green", Blue = "blue";
    private const int MaxRed = 12, MaxGreen = 13, MaxBlue = 14;

    private readonly List<Game> _games = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var lineSplit = line.Split(": ");
            var gameId = int.Parse(lineSplit[0].Split(' ')[^1]);
            var game = new Game(gameId);
            ParseGame(lineSplit[1], ref game);
            _games.Add(game);
        }
    }

    public object SolvePart1() => _games.Sum(g => g.IsPossible() ? g.Id : 0);

    public object SolvePart2() => _games.Sum(g => g.Power);

    private static void ParseGame(string line, ref Game game)
    {
        var matches = Regex.Matches(line, @"(\d+ red)|(\d+ green)|(\d+ blue)");
        foreach (Match match in matches)
        {
            var split = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var amount = int.Parse(split[0]);
            switch (split[1])
            {
                case Red: game.RedRevealed(amount); break;
                case Green: game.GreenRevealed(amount); break;
                case Blue: game.BlueRevealed(amount); break;
            }
        }
    }

    private record struct Game(int Id)
    {
        private int _red, _green, _blue;
        public int Power => _red * _green * _blue;
        public bool IsPossible() => _red <= MaxRed && _green <= MaxGreen && _blue <= MaxBlue;

        public void RedRevealed(int red) => _red = Math.Max(red, _red);
        public void GreenRevealed(int green) => _green = Math.Max(green, _green);
        public void BlueRevealed(int blue) => _blue = Math.Max(blue, _blue);
    }
}