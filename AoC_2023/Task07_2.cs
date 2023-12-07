using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task07_2
    {
        [Test]
        [TestCase(
            @"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483",
            5905)]
        [TestCase(@"Task07.txt", 250384185)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var lines = input.SplitLines()
                .Select(x => (Hand: x.SplitEmpty(" ")[0]
                        .Replace("A", "E")
                        .Replace("K", "D")
                        .Replace("Q", "C")
                        .Replace("J", "0")
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
                var jCnt = hand.Count(c => c == '0');

                var grouped = hand.Where(c => c != '0').GroupBy(c => c).ToDictionary(c => c.Key, c => c.Count());
                
                if (jCnt == 0)
                {
                    if (grouped.Values.Max() == 5) return FiveHand;
                    if (grouped.Values.Max() == 4) return FourHand;
                    if (grouped.Any(x => x.Value == 3) && grouped.Any(x => x.Value == 2)) return FullHouseHand;
                    if (grouped.Any(x => x.Value == 3)) return ThreeHand;
                    if (grouped.Count(x => x.Value == 2) == 2) return TwoPairHand;
                    if (grouped.Any(x => x.Value == 2)) return OnePairHand;
                    return HighCardHand;
                }
                
                switch (jCnt)
                {
                    case 5: return FiveHand;
                    case 4: return FiveHand;
                    case 3: return grouped.Any(x => x.Value == 2) ? FiveHand : FourHand;
                    case 2:
                        return grouped.Any(x => x.Value == 3)
                            ? FiveHand
                            : grouped.Any(x => x.Value == 2)
                                ? FourHand
                                : ThreeHand;
                    case 1: return grouped.Any(x => x.Value == 4)
                        ? FiveHand
                        : grouped.Any(x => x.Value == 3)
                            ? FourHand
                            : grouped.All(x => x.Value == 2)
                            ? FullHouseHand
                            : grouped.Any(x=>x.Value == 2)
                            ? ThreeHand
                            : OnePairHand
                            ;
                }

                throw new NotImplementedException();
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