using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task14
    {
        [Test]
        [TestCase(
            @"O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....",
            136)]
        [TestCase(@"Task14.txt", 109385)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines();
            
            var result = 0;

            var bases = new int[map[0].Length];
            for (var i = 0; i < map.Length; ++i)
            for (var j = 0; j < map[0].Length; ++j)
            {
                var current = map[i][j];
                if (current == '.') continue;

                if (current == 'O')
                {
                    result += map.Length - bases[j];
                    bases[j]++;
                }
                else
                {
                    bases[j] = i + 1;
                }
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