using System.Collections;
using System.Diagnostics;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task23
    {
        [Test]
        [TestCase(
            @"#.#####################
#.......#########...###
#######.#########.#.###
###.....#.>.>.###.#.###
###v#####.#v#.###.#.###
###.>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>.#
#.#.#v#######v###.###v#
#...#.>.#...>.>.#.###.#
#####v#.#.###v#.#.###.#
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>.>.#.>.###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################.#",
            94)]
        [TestCase(@"Task23.txt", 0)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

            var visited = new HashSet<(int Row, int Column)>();
            var paths = new Dictionary<(int Row, int Column), int>();
            var startColumn = map[0].Select((c, i) => (c, i)).Single(x => x.c == '.').i;
            var endColumn = map.Last().Select((c, i) => (c, i)).Single(x => x.c == '.').i;
            Dfs(visited, paths, map, (Row: 0, Column: startColumn), (Row: map.Length - 1, Column: endColumn), 0);

            var result = paths[(map.Length - 1, endColumn)];
            result.Should().Be(expected);
        }

        private void Dfs(HashSet<(int Row, int Column)> visited, Dictionary<(int Row, int Column), int> pathLengths,
            char[][] map, (int Row, int Column) currentIndex, (int Row, int Column) endIndex, int pathLength)
        {
            var s = Print(map, visited);
            TestContext.Out.WriteLine(s);
            TestContext.Out.WriteLine();
            
            if (visited.Contains(currentIndex)) return;

            if (!pathLengths.ContainsKey(currentIndex) || pathLengths[currentIndex] < pathLength)
            {
                pathLengths[currentIndex] = pathLength;
            }

            if (currentIndex == endIndex) return;

            var currentItem = map[currentIndex.Row][currentIndex.Column];

            if (directions.TryGetValue(currentItem, out var dir))
            {
                visited.Add(currentIndex);
                Dfs(visited, pathLengths, map, (currentIndex.Row + dir.Item1, currentIndex.Column + dir.Item2),
                    endIndex, pathLength + 1);
                visited.Remove(currentIndex);
                return;
            }

            foreach (var neighbour in Extensions.GetVerticalHorizontalNeighbours(map, currentIndex))
            {
                if (neighbour.Item != '.' && !directions.ContainsKey(neighbour.Item)) continue;

                visited.Add(currentIndex);
                Dfs(visited, pathLengths, map, neighbour.Index, endIndex, pathLength + 1);
                visited.Remove(currentIndex);
            }
        }

        private static string Print<T>(T[][] map, HashSet<(int, int)> visited)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (visited.Contains((i, j))) sb.Append('O');
                    else sb.Append(map[i][j]);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
        

        private static Dictionary<char, (int, int)> directions = new Dictionary<char, (int, int)>
        {
            { '>', (0, 1) },
            { 'v', (1, 0) },
        };

        private static (int Row, int Column)[] Directions = new[]
        {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0),
        };
    }
}