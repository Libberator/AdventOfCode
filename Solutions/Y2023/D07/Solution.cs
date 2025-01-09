using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC.Solutions.Y2023.D07;

public class Solution : ISolver
{
    private static readonly char[] Ranking = ['2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'];
    private static readonly char[] JokerRanking = ['J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A'];
    private readonly List<Hand> _hands = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
        {
            var split = line.Split(' ');
            var hand = new Hand(split[0], int.Parse(split[1]));
            _hands.Add(hand);
        }
    }

    public object SolvePart1() => GetTotalWinnings(_hands
        .OrderBy(h => h.HandType).ThenBy(h => h.HandScore));

    public object SolvePart2() => GetTotalWinnings(_hands
        .OrderBy(h => h.JokerHandType).ThenBy(h => h.JokerHandScore));

    private static int GetTotalWinnings(IEnumerable<Hand> hands)
    {
        var rank = 1;
        return hands.Aggregate(0, (total, h) => total + h.Bid * rank++);
    }

    private enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    private readonly record struct Hand()
    {
        public readonly HandType HandType, JokerHandType;
        public readonly int Bid, HandScore, JokerHandScore;

        public Hand(string cards, int bid) : this()
        {
            Bid = bid;
            HandType = GetHandType(cards);
            JokerHandType = GetJokerHandType(cards);
            HandScore = GetHandScore(cards, Ranking);
            JokerHandScore = GetHandScore(cards, JokerRanking);
        }

        private static HandType GetHandType(string cards) => cards.ToHashSet().Count switch
        {
            1 => HandType.FiveOfAKind,
            2 => cards.Count(c => c == cards[0]) is 1 or 4 ? HandType.FourOfAKind : HandType.FullHouse,
            3 => cards.Any(i => cards.Count(c => c == i) == 3) ? HandType.ThreeOfAKind : HandType.TwoPair,
            4 => HandType.OnePair,
            _ => HandType.HighCard
        };

        private static HandType GetJokerHandType(string cards) => cards.Count(c => c == 'J') switch
        {
            4 or 5 => HandType.FiveOfAKind,
            3 => cards.ToHashSet().Count is 2 ? HandType.FiveOfAKind : HandType.FourOfAKind,
            2 => cards.ToHashSet().Count switch
            {
                2 => HandType.FiveOfAKind,
                3 => HandType.FourOfAKind,
                _ => HandType.ThreeOfAKind
            },
            1 => cards.ToHashSet().Count switch
            {
                2 => HandType.FiveOfAKind,
                3 => cards.Count(c => c == cards.First(i => i != 'J')) is 1 or 3
                    ? HandType.FourOfAKind
                    : HandType.FullHouse,
                4 => HandType.ThreeOfAKind,
                _ => HandType.OnePair
            },
            _ => GetHandType(cards)
        };

        private static int GetHandScore(string cards, char[] ranking)
        {
            var score = 0;
            foreach (var card in cards)
            {
                score *= 100;
                score += Array.IndexOf(ranking, card);
            }

            return score;
        }
    }
}