using System.Collections;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task21
    {
        [Test]
        [TestCase(
            @"...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........",
            16, 6)]
        [TestCase(@"Task21.txt", 3660, 64)]
        public void Task(string input, long expected, int stepsCount)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

            var start = (Row: 0, Col: 0);
            for (var i = 0; i < map.Length; i++)
            for (var j = 0; j < map[0].Length; j++)
            {
                if (map[i][j] == 'S')
                {
                    start = (i, j);
                    break;
                }
            }

            var steps = new HashSet<(int Row, int Column)>();
            steps.Add(start);
            for (var step = 0; step < stepsCount; ++step)
            {
                var newSteps = new HashSet<(int Row, int Column)>();

                foreach (var current in steps)
                {
                    var newInds = Extensions.GetVerticalHorizontalNeighbours(map, current).Where(x => x.Item != '#')
                        .Select(x => x.Index);
                    foreach (var newInd in newInds)
                    {
                        newSteps.Add(newInd);
                    }
                }

                steps = newSteps;
            }

            var result = (long)steps.Count;
            result.Should().Be(expected);
        }
    }
}