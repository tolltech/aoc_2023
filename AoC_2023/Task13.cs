using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task13
    {
        [Test]
        [TestCase(
            @"#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.

#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#",
            405)]
        [TestCase(@"Task13.txt", 34772)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var result = 0;

            var maps = input.SplitEmpty("\r\n\r\n", "\n\n")
                .Select(x => x.SplitLines());
            foreach (var map in maps)
            {
                var rows = map.ToArray();
                var columns = new List<string>();

                for (var i = 0; i < map[0].Length; ++i)
                {
                    var column = new string(map.Select(x => x[i]).ToArray());
                    columns.Add(column);
                }

                var rowMirror = GetMirror(rows);
                var columnMirror = GetMirror(columns.ToArray());
                if (rowMirror != -1) result += 100 * rowMirror;
                else if (columnMirror != -1) result += columnMirror;
                else throw new NotImplementedException();
            }

            result.Should().Be(expected);
        }

        private int GetMirror(string[] rows)
        {
            for (var i = 1; i < rows.Length; ++i)
            {
                var left = i - 1;
                var right = i;
                if (CheckMirror(left, right, rows)) return i;
            }

            return -1;
        }

        private bool CheckMirror(int left, int right, string[] rows)
        {
            while (true)
            {
                if (rows[left] != rows[right]) return false;

                if (left == 0 || right == rows.Length - 1)
                {
                    break;
                }

                left--;
                right++;
            }

            return true;
        }
    }
}