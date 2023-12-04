﻿using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task04_2
    {
        [Test]
        [TestCase(
            @"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11",
            30)]
        [TestCase(@"Task04.txt", 6227972)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;
            
            var result = 0;

            var lines = input.SplitLines();
            
            var cardsCount = new Dictionary<int, int>();
            for (var i = 1; i <= lines.Length; ++i) cardsCount[i] = 1;

            var current = 1;
            foreach (var line in lines)
            {
                var sets = line.SplitEmpty(":")[1].SplitEmpty("|").Select(x => x.SplitEmpty(" ").ToHashSet()).ToArray();
                var cnt = sets[1].Count(sets[0].Contains);

                for (var i = 1; i <= cnt; ++i)
                {
                    cardsCount[current + i] += cardsCount[current];
                }

                ++current;
            }

            cardsCount.Values.Sum().Should().Be(expected);

        }
    }
}