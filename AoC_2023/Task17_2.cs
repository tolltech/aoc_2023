using System.Collections;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task17_2
    {
        [Test]
        [TestCase(
            @"2413432311323
3215453535623
3255245654254
3446585845452
4546657867536
1438598798454
4457876987766
3637877979653
4654967986887
4564679986453
1224686865563
2546548887735
4322674655533",
            94)]
        [TestCase(
            @"111111111111
999999999991
999999999991
999999999991
999999999991",
            71)]
        [TestCase(@"Task17.txt", 822)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines().Select(x => x.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

            var distances = Dijkstra(map);

            //var s = Print(distances, map);

            var result = distances[(map.Length - 1, map[0].Length - 1)]
                .Where(x => new string(x.Key.TakeLast(4).ToArray()).Distinct().Count() == 1)
                .Min(x => x.Value);

            result.Should().Be(expected);
        }

        private (string, string) Print(Dictionary<(int, int), int> distances, int[][] map)
        {
            var sb = new StringBuilder();
            var sb2 = new StringBuilder();

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    sb.Append($"|{distances[(i, j)].ToString().PadLeft(5, '_')}|");
                    sb2.Append($"|{map[i][j].ToString().PadLeft(5, '_')}|");
                }

                sb.AppendLine();
                sb2.AppendLine();
            }

            return (sb.ToString(), sb2.ToString());
        }

        private Dictionary<(int, int), Dictionary<string, int>> Dijkstra(int[][] map)
        {
            var dist = new Dictionary<(int Row, int Column, string Directions), int>();
            var marked = new HashSet<(int, int, string)>();

            var pq = new PriorityQueue<(int Row, int Column, string PrevDirections), int>();

            dist[(0, 0, string.Empty)] = 0;

            pq.Enqueue((0, 0, string.Empty), 0);
            while (pq.Count > 0)
            {
                var v = pq.Dequeue();
                if (marked.Contains(v)) continue;
                marked.Add(v);

                var nextVs = GetNextV(map, v, marked);

                foreach (var nextV in nextVs)
                {
                    var weight = nextV is { Row: 0, Column: 0 } ? 0 : map[nextV.Row][nextV.Column];
                    if (!dist.ContainsKey(nextV) || dist[nextV] > dist[v] + weight)
                    {
                        dist[nextV] = dist[v] + weight;
                        pq.Enqueue(nextV, dist[nextV]);
                    }
                }
            }

            return dist.GroupBy(x => (x.Key.Row, x.Key.Column))
                .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Key.Directions, y => y.Value));
        }

        private IEnumerable<(int Row, int Column, string PrevDirections)> GetNextV(int[][] map,
            (int Row, int Column, string PrevDirections) current, HashSet<(int, int, string)> marked)
        {
            foreach (var next in Extensions.GetVerticalHorizontalNeighboursDirections(map, (current.Row, current.Column)))
            {
                if (current.PrevDirections.GetLastOrEmpty() == opposites[codes[next.Direction]])
                    continue;
                
                var nextP = (next.Index.Row, next.Index.Col,
                    new string(current.PrevDirections.TakeLast(9).ToArray()) + codes[next.Direction]);

                if (marked.Contains(nextP)) continue;
                
                if (current.PrevDirections.Length == 10 && current.PrevDirections.Distinct().Count() == 1
                                                       && current.PrevDirections.Distinct().Single().ToString() == codes[next.Direction])
                    continue;

                var last4 = new string(current.PrevDirections.TakeLast(4).ToArray());
                if (last4.Length > 0 && (last4.Length != 4 || last4.Distinct().Count() != 1) && last4.Last().ToString() != codes[next.Direction])
                    continue;

                yield return nextP;
            }
        }


        public static readonly (int Row, int Column) DownStep = (1, 0);
        public static readonly (int Row, int Column) LeftStep = (0, -1);
        public static readonly (int Row, int Column) UpStep = (-1, 0);
        public static readonly (int Row, int Column) RightStep = (0, 1);

        private static readonly Dictionary<(int, int), string> codes = new Dictionary<(int, int), string>
        {
            { DownStep, "d" },
            { UpStep, "u" },
            { LeftStep, "l" },
            { RightStep, "r" },
        };

        private static readonly Dictionary<string, string> opposites = new Dictionary<string, string>
        {
            { "u", "d" },
            { "d", "u" },
            { "r", "l" },
            { "l", "r" },
        };

        private static readonly Dictionary<string, (int, int)> directions = new Dictionary<string, (int, int)>
        {
            { "d", DownStep },
            { "u", UpStep },
            { "l", LeftStep },
            { "r", RightStep },
        };
    }
}