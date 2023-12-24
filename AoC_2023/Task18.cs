using System.Collections;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task18
    {
        [Test]
        [TestCase(
            @"R 6 (#70c710)
D 5 (#0dc571)
L 2 (#5713f0)
D 2 (#d2c081)
R 2 (#59c680)
D 2 (#411b91)
L 5 (#8ceee2)
U 2 (#caa173)
L 1 (#1b58a2)
U 2 (#caa171)
R 2 (#7807d2)
U 3 (#a77fa3)
L 2 (#015232)
U 2 (#7a21e3)",
            62)]
        [TestCase(@"Task18.txt", 53844)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;
            
            var result = 0;

            var points = new List<(int Row, int Column)>();

            points.Add((0, 0));
            
            foreach (var line in input.SplitLines())
            {
                var splits = line.SplitEmpty(" ");
                var direction = splits[0];
                var val = int.Parse(splits[1]);
                var color = splits[2];

                var step = directions[direction];
                var lastPoint = points.Last();
                points.Add((lastPoint.Row + step.Item1 * val, lastPoint.Column + step.Item2 * val));
            }

            var minX = points.Select(x => x.Column).Min();
            var maxX = points.Select(x => x.Column).Max();
            var minY = points.Select(x => x.Row).Min();
            var maxY = points.Select(x => x.Row).Max();

            var totalX = maxX - minX + 1;
            var totalY = maxY - minY + 1;

            var map = new char[totalY][];
            for (var i = 0; i < map.Length; i++)
            {
                map[i] = Enumerable.Repeat('.', totalX).ToArray();
            }

            var deltaX = -minX;
            var deltaY = -minY;

            var point = (Row: deltaY, Column: deltaX);
            map[point.Row][point.Column] = '#';
            foreach (var line in input.SplitLines())
            {
                var splits = line.SplitEmpty(" ");
                var direction = splits[0];
                var val = int.Parse(splits[1]);
                var color = splits[2];

                var step = directions[direction];
                for (var i = 0; i < val; ++i)
                {
                    point = (point.Row + step.Item1, point.Column + step.Item2);
                    map[point.Row][point.Column] = '#';
                }
            }

            var s = Print(map);

            var randomPointInside = (1, map[0].Select(((c, i) => (c, i))).SkipWhile(c => c.c == '.').Skip(1).First().i);
            var queue = new Queue<(int Row, int Column)>();

            queue.Enqueue(randomPointInside);

            var visitedPoints = new HashSet<(int Row, int Column)>();
            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                if (visitedPoints.Contains(p)) continue;

                visitedPoints.Add(p);

                foreach (var neighbour in Extensions.GetAllNeighbours(map, p))
                {
                    if (neighbour.Item == '#') continue;
                    if (visitedPoints.Contains(neighbour.Index)) continue;
                    queue.Enqueue(neighbour.Index);
                }
            }

            (visitedPoints.Count + map.Sum(row => row.Count(c => c == '#'))).Should().Be(expected);
        }

        private string Print(char[][] map)
        {
            var sb2 = new StringBuilder();

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    sb2.Append(map[i][j]);
                }

                sb2.AppendLine();
            }

            return sb2.ToString();
        }
        
        public static readonly (int Row, int Column) DownStep = (1, 0);
        public static readonly (int Row, int Column) LeftStep = (0, -1);
        public static readonly (int Row, int Column) UpStep = (-1, 0);
        public static readonly (int Row, int Column) RightStep = (0, 1);
        
        private static readonly Dictionary<string, (int, int)> directions = new Dictionary<string, (int, int)>
        {
            { "D", DownStep },
            { "U", UpStep },
            { "L", LeftStep },
            { "R", RightStep },
        };
    }
}