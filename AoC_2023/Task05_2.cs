using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task05_2
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
            46)]
        //[TestCase(@"Task05.txt", 84206669)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var splits = input.SplitEmpty("\r\n\r\n");

            TestContext.Out.WriteLine($"DONE {splits.Length}%");

            var inputSeeds = splits[0].SplitEmpty(":")[1].SplitEmpty(" ").Select(long.Parse).ToArray();
            var seeds = new List<(long From, long To)>();
            for (var i = 0; i < inputSeeds.Length; i += 2)
            {
                var start = inputSeeds[i];
                var range = inputSeeds[i + 1];

                seeds.Add((start, start + range));
            }

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
                            DestinationFrom = numbers[0],
                            DestinationTo = numbers[0] + numbers[2],
                            Range = numbers[2]
                        };
                    }).ToArray();
                })
                .ToList();

            var result = long.MaxValue;

            seeds = Merge(seeds);

            foreach (var mergedSeed in seeds)
            {
                for (var seed = mergedSeed.From; seed < mergedSeed.To; seed++)
                {
                    var progress = (int)((seed - mergedSeed.From) * 100.0 / (mergedSeed.To - mergedSeed.From));
                    if (seed % 1000000 == 0)
                        TestContext.Out.WriteLine($"DONE {progress}%");

                    if (!seeds.Any(x => x.From <= seed && x.To > seed)) continue;

                    var currentSeed = seed;
                    foreach (var step in map)
                    {
                        var suitableMap = step.FirstOrDefault(x => x.SourceFrom <= currentSeed && x.SourceTo > currentSeed);
                        if (suitableMap != null)
                        {
                            currentSeed = currentSeed - suitableMap.SourceFrom + suitableMap.DestinationFrom;
                        }
                    }

                    var newResult = currentSeed;

                    if (newResult < result) result = newResult;
                }
    
            }
            
            result.Should().Be(expected);
        }

        private List<(long From, long To)> Merge(List<(long From, long To)> seeds)
        {
            var sortedSeeds = new Queue<(long From, long To)>(seeds.OrderBy(x => x.From));

            var result = new List<(long From, long To)>();
            result.Add(sortedSeeds.Dequeue());

            while (true)
            {
                if (sortedSeeds.Count == 0) break;
                var newSeed = sortedSeeds.Dequeue();
                var merged = Merge(result.Last(), newSeed);
                if (merged.Length == 2)
                {
                    result.Add(newSeed);
                }
                else
                {
                    result.RemoveAt(result.Count - 1);
                    result.Add(merged.Single());
                }
            }

            return result;
        }

        private (long From, long To)[] Merge((long From, long To) left, (long From, long To) right)
        {
            var sorted = new[] { left, right }.OrderBy(x => x.From).ToArray();
            left = sorted[0];
            right = sorted[1];

            if (left.To < right.From) return new[] { left, right };
            return new[] { (left.From, Math.Max(right.To, left.To)) };
        }
    }
}