using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task05
    {
        [Test]
        [TestCase(
            @"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4",
            35)]
        [TestCase(@"Task05.txt", 388071289)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var splits = input.SplitEmpty("\r\n\r\n");

            var seeds = splits[0].SplitEmpty(":")[1].SplitEmpty(" ").Select(long.Parse).ToArray();

            var map = splits.Skip(1)
                .Select(x =>
                {
                    var lines = x.Split(":")[1].SplitEmpty("\r", "\n");
                    return lines.Select(l =>
                    {
                        var numbers = l.SplitEmpty(" ").Select(long.Parse).ToArray();
                        return new
                        {
                            SourceFrom = numbers[1],
                            SourceTo = numbers[1] + numbers[2],
                            Destination = numbers[0],
                            Range = numbers[2]
                        };
                    }).ToArray();
                })
                .ToList();

            var result = long.MaxValue;

            foreach (var seed in seeds)
            {
                var currentSeed = seed;
                foreach (var step in map)
                {
                    var suitableMap = step.FirstOrDefault(x => x.SourceFrom <= currentSeed && x.SourceTo > currentSeed);
                    if (suitableMap != null)
                    {
                        currentSeed = currentSeed - suitableMap.SourceFrom + suitableMap.Destination;
                    }
                }

                var newResult = currentSeed;
                
                if (newResult < result) result = newResult;
            }
            
            result.Should().Be(expected);
        }
    }
}