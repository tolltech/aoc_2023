using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task13_2
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
            400)]
        [TestCase(@"Task13.txt", 35554)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var result = 0;

            var maps = input.SplitEmpty("\r\n\r\n", "\n\n")
                .Select(x => x.SplitLines().Select(y => y.ToArray()).ToArray());
            foreach (var map in maps)
            {
                var rows = map.Select(x => new string(x)).ToArray();
                var columns = new List<string>();

                for (var i = 0; i < map[0].Length; ++i)
                {
                    var column = new string(map.Select(x => x[i]).ToArray());
                    columns.Add(column);
                }

                var oldRowMirror = GetMirror(rows);
                var oldColumnMirror = GetMirror(columns.ToArray());

                foreach (var mutatedMapTuple in Mutate(map))
                {
                    var mutatedMap = mutatedMapTuple.Item1;
                    var rowMustInclude = mutatedMapTuple.Item2.Item1;
                    var columnMustInclude = mutatedMapTuple.Item2.Item2;
                    rows = mutatedMap.Select(x => new string(x)).ToArray();
                    columns = new List<string>();

                    for (var i = 0; i < mutatedMap[0].Length; ++i)
                    {
                        var column = new string(mutatedMap.Select(x => x[i]).ToArray());
                        columns.Add(column);
                    }

                    var rowMirror = GetMirror(rows, rowMustInclude);
                    var columnMirror = GetMirror(columns.ToArray(), columnMustInclude);
                    if (rowMirror != -1) result += 100 * rowMirror;
                    else if (columnMirror != -1) result += columnMirror;
                    else continue;

                    break;
                }
            }

            result.Should().Be(expected);
        }

        private IEnumerable<(char[][], (int, int))> Mutate(char[][] map)
        {
            for (var i = 0; i < map.Length; ++i)
            for (var j = 0; j < map[0].Length; ++j)
            {
                var tmp = map[i][j];
                map[i][j] = tmp == '#' ? '.' : '#';
                yield return (map.Select(x => x.ToArray()).ToArray(), (i, j));
                map[i][j] = tmp;
            }
        }

        private int GetMirror(string[] rows, int mustInclude = -1)
        {
            for (var i = 1; i < rows.Length; ++i)
            {
                var left = i - 1;
                var right = i;

                if (CheckMirror(left, right, rows, mustInclude)) return i;
            }

            return -1;
        }

        private bool CheckMirror(int left, int right, string[] rows, int mustInclude)
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

            if (mustInclude == -1)
                return true;

            return mustInclude >= left && mustInclude <= right;
        }
    }
}