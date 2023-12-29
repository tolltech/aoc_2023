using System.Collections;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task21_2
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
            50, 10)]
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
            1594, 50)]
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
            16733044, 5000)]
        [TestCase(@"Task21.txt", 0, 26501365)]
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
                    foreach (var dir in Directions)
                    {
                        var newInd = (current.Row + dir.Row, current.Column + dir.Column);
                        var item = GetInfinityItem(map, newInd);
                        if (item.Item == '#') continue;

                        newSteps.Add(newInd);
                    }
                }

                steps = newSteps;
            }

            var result = (long)steps.Count;
            result.Should().Be(expected);
        }

        private static (int Row, int Column)[] Directions = new[]
        {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0),
        };

        private static (T Item, (int Row, int Col) NormIdx) GetInfinityItem<T>(T[][] map, (int Row, int Col) index)
        {
            var row = (index.Row + map.Length * 100000000L) % map.Length;
            var col = (index.Col + map[0].Length * 100000000L) % map[0].Length;
            return (map[row][col], ((int)row, (int)col));
        }
    }
}