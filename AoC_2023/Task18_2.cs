using System.Collections;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task18_2
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
            952408144115L)]
        [TestCase(@"Task18.txt", 42708339569950L)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var result = 0L;

            var points = new List<(long Row, long Column)>();

            points.Add((0, 0));

            foreach (var line in input.SplitLines())
            {
                var splits = line.SplitEmpty(" ");
                var color = splits[2].Trim('(', ')', '#');

                var direction = color.Last().ToString()!;
                var val = color.Substring(0, color.Length - 1).ToUpper().ToBinFromHex().ToLongFromBin();

                var step = directions[direction];
                var lastPoint = points.Last();
                var newPoint = (lastPoint.Row + step.Item1 * val, lastPoint.Column + step.Item2 * val);
                points.Add(newPoint);

                result += val;
            }

            var tmp = 0L;
            for (var i = 0; i < points.Count - 1; ++i)
            {
                tmp += points[i].Column * points[i + 1].Row;
            }

            tmp += points.Last().Column * points.First().Row;
            
            for (var i = 0; i < points.Count - 1; ++i)
            {
                tmp -= points[i + 1].Column * points[i].Row;
            }

            tmp -= points.First().Column * points.Last().Row;

            tmp = Math.Abs(tmp);

            ((tmp + result) / 2 + 1).Should().Be(expected);
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
            { "1", DownStep },
            { "3", UpStep },
            { "2", LeftStep },
            { "0", RightStep },
        };
    }
}