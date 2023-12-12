using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task11_2
    {
        [Test]
        [TestCase(
            @"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....",
            10, 1030)]
        [TestCase(
            @"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....",
            100, 8410)]
        [TestCase(@"Task11.txt", 2, 9509330)]
        [TestCase(@"Task11.txt", 1000000, 635832237682L)]
        public void Task(string input, long mult, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines().Select(x => x.ToArray()).ToArray();
            var xGalaxies = new HashSet<int>();
            var yGalaxies = new HashSet<int>();
            var galaxies = new List<(int Row, int Column, int Number)>();
            var number = 1;
            for (var i = 0; i < map.Length; i++)
            for (var j = 0; j < map[0].Length; j++)
            {
                if (map[i][j] != '#') continue;

                xGalaxies.Add(j);
                yGalaxies.Add(i);
                galaxies.Add((i, j, number++));
            }

            var result = 0L;
            for (var i = 0; i < galaxies.Count; ++i)
            for (var j = i + 1; j < galaxies.Count; j++)
            {
                var pair = new[] { galaxies[i], galaxies[j] };

                var pairNumbers = pair.Select(x => x.Number).OrderBy(x => x).ToArray();

                var pairX = pair.OrderBy(x => x.Column).ToArray();
                var xGalaxyCount = xGalaxies.Count(x => x > pairX.First().Column && x <= pairX.Last().Column);
                var deltaX = pairX.Last().Column - pairX.First().Column;
                result += deltaX * mult + (1 - mult) * xGalaxyCount;

                var pairY = pair.OrderBy(x => x.Row).ToArray();
                var yGalaxyCount = yGalaxies.Count(x => x > pairY.First().Row && x <= pairY.Last().Row);
                var deltaY = pairY.Last().Row - pairY.First().Row;
                result += deltaY * mult + (1 - mult) * yGalaxyCount;
            }

            result.Should().Be(expected);
        }
    }
}