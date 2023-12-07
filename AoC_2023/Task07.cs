using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task07
    {
        [Test]
        [TestCase(
            @"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483",
            6440)]
        [TestCase(@"Task07.txt", 251545216)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var lines = input.SplitLines()
                .Select(x => (Hand: x.SplitEmpty(" ")[0]
                        .Replace("A", "E")
                        .Replace("K", "D")
                        .Replace("Q", "C")
                        .Replace("J", "B")
                        .Replace("T", "A")
                    , Rank: long.Parse(x.SplitEmpty(" ")[1])))
                .ToArray();

            var result = 0L;

            var hands = lines.OrderBy(x => x, new HandComparer()).ToArray();
            result = hands.Select((tuple, i) => tuple.Rank * (i + 1)).Sum();
            
            result.Should().Be(expected);
        }

        class HandComparer : IComparer<(string, long)>
        {
            public int Compare((string, long) x, (string, long) y)
            {
                var typeX = GetHandType(x.Item1);
                var typeY = GetHandType(y.Item1);

                if (typeX != typeY) return typeX - typeY;

                return String.CompareOrdinal(x.Item1, y.Item1);
            }

            private int GetHandType(string hand)
            {
                var grouped = hand.GroupBy(c => c).ToDictionary(c => c.Key, c => c.Count());
                if (grouped.Any(x => x.Value == 5)) return FiveHand;
                if (grouped.Any(x => x.Value == 4)) return FourHand;
                if (grouped.Any(x => x.Value == 3) && grouped.Any(x => x.Value == 2)) return FullHouseHand;
                if (grouped.Any(x => x.Value == 3)) return ThreeHand;
                if (grouped.Count(x => x.Value == 2) == 2) return TwoPairHand;
                if (grouped.Any(x => x.Value == 2)) return OnePairHand;

                return HighCardHand;
            }
        }

        private const int FiveHand = 7;
        private const int FourHand = 6;
        private const int FullHouseHand = 5;
        private const int ThreeHand = 4;
        private const int TwoPairHand = 3;
        private const int OnePairHand = 2;
        private const int HighCardHand = 1;
    }
}