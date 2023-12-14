using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task14_2
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
            64)]
        [TestCase(@"Task14.txt", 93102)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

            var result = 0L;
            var caches = new Dictionary<string, int>();
            var cycleList = new List<long>();
            var index = 0;
            var wasRepeat = 0;
            string mapKey;
            while (true)
            {
                mapKey = map.Select(x => new string(x)).JoinToString("\r\n");
                if (caches.ContainsKey(mapKey))
                {
                    break;
                }
                // {
                //     if (wasRepeat > 20)
                //         break;
                //
                //     wasRepeat++;
                // }

                caches[mapKey] = index;
                cycleList.Add(Calculate(map));

                for (var i = 0; i < 4; ++i)
                {
                    Gravitate(map);
                    map = Rotate(map);
                }

                index++;
            }

            var prefix = caches[mapKey];
            var cutCycle = cycleList.Skip(prefix).ToArray();

            cutCycle[(1000000000 - prefix) % cutCycle.Length].Should().Be(expected);
        }

        private long Calculate(char[][] map)
        {
            var result = 0L;
            for (var i = 0; i < map.Length; ++i)
            {
                result += map[i].Count(x => x == 'O') * (map.Length - i);
            }

            return result;
        }

        private static char[][] Rotate(char[][] matrix)
        {
            var b = matrix.Select(x => x.ToArray()).ToArray();
            for (int i = 0; i < b.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    b[j][i] = matrix[matrix.Length - i - 1][j];
                }
            }

            return b;
        }

        private static int Gravitate(char[][] map)
        {
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

                    if (bases[j] != i)
                    {
                        map[bases[j]][j] = current;
                        map[i][j] = '.';
                    }

                    bases[j]++;
                }
                else
                {
                    bases[j] = i + 1;
                }
            }

            return result;
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