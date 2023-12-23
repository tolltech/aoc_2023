using System.Collections;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task17
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
            102)]
        [TestCase(@"Task17.txt", 0)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines().Select(x => x.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

            var distances = Dijkstra(map);

            var s = Print(distances, map);

            var result = distances[(map.Length - 1, map[0].Length - 1)];
            
            result.Should().Be(expected);
        }

        private (string, string) Print(Dictionary<(int, int),int> distances, int[][] map)
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

        private Dictionary<(int, int), int> Dijkstra(int[][] map)
        {
            var dist = new Dictionary<(int,int), int>();
            var marked = new HashSet<(int, int)>();

            var pq = new PriorityQueue<((int Row, int Column) Index, Stack<(int,int)> PrevDirections), int>();

            dist[(0, 0)] = 0;
            
            pq.Enqueue(((0,0), new Stack<(int, int)>()), 0);
            while (pq.Count > 0)
            {
                var v = pq.Dequeue();
                if (marked.Contains(v.Index)) continue;
                marked.Add(v.Index);

                var nextVs = GetNextV(map, v, marked);
                
                foreach (var nextV in nextVs)
                {
                    var weight = map[nextV.Index.Row][nextV.Index.Column];
                    if (!dist.ContainsKey(nextV.Index) || dist[nextV.Index] > dist[v.Index] + weight)
                    {
                        dist[nextV.Index] = dist[v.Index] + weight;
                        pq.Enqueue(nextV, dist[nextV.Index]);
                    }
                }
            }

            return dist;
        }
        
        private IEnumerable<((int Row, int Column) Index, Stack<(int,int)> PrevDirections)> GetNextV(int[][] map, ((int Row, int Column) Index, Stack<(int,int)> PrevDirections) current, HashSet<(int, int)> marked)
        {
            foreach (var next in Extensions.GetVerticalHorizontalNeighboursDirections(map, current.Index))
            {
                if (marked.Contains(next.Index)) continue;
                
                if (current.PrevDirections.Count == 3 && current.PrevDirections.Distinct().Count() == 1
                    && current.PrevDirections.Distinct().Single() == next.Direction) continue;

                var prevDirections = new Stack<(int, int)>();
                var prev = current.PrevDirections.Take(2).Reverse();
                foreach (var p in prev)
                {
                    prevDirections.Push(p);                    
                }

                prevDirections.Push(next.Direction);
                
                yield return (next.Index, prevDirections);
            }
        }
        
        
        public static readonly (int Row, int Column) DownStep = (1, 0);
        public static readonly (int Row, int Column) LeftStep = (0, -1);
        public static readonly (int Row, int Column) UpStep = (-1, 0);
        public static readonly (int Row, int Column) RightStep = (0, 1);

        private static readonly Dictionary<(int, int), string> codes = new Dictionary<(int, int), string>
        {
            {DownStep, "d"},
            {UpStep, "u"},
            {LeftStep, "l"},
            {RightStep, "r"},
        };
        
        private static readonly Dictionary<string, (int, int)> directions = new Dictionary<string, (int, int)>
        {
            {"d", DownStep},
            {"u", UpStep},
            {"l", LeftStep},
            {"r", RightStep},
        };
    }
}