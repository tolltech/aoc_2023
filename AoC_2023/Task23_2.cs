using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task23_2
    {
        [Test]
        [TestCase(
            @"#a#####################
#.......#########...###
#######.#########.#.###
###.....#.>c>.###.#.###
###v#####.#v#.###.#.###
###b>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>e#
#.#.#v#######v###.###v#
#...#f>.#...>d>.#.###.#
#####v#.#.###v#.#.###.#
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>h>.#.>g###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################z#",
            154)]
        [TestCase(@"Task23.txt", 6450)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;
            input = input.Replace(">", ".").Replace("v", ".");
            var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

            var root = BuildGraph(map, out var nodes);

            var max = DfsNode(new HashSet<Node>(), root, 0);
            
            max.Should().Be(expected);
        }

        private static int globalMax = -1;

        // private IEnumerable<int> CheckLength(Node root, int nodesCount)
        // {
        //     var cnt = (long)Math.Pow(2, nodesCount - 4);
        //     for (long mask = cnt - 1; mask >= 0; --mask)
        //     {
        //         var maskStr = $"11{mask.ToString("X").ToBinFromHex().PadLeft(nodesCount - 4, '0')}11";
        //
        //         var max = DfsNode(new HashSet<Node>(), root, 0, maskStr);
        //
        //         if (max > globalMax)
        //             globalMax = max;
        //
        //         yield return max;
        //     }
        // }

        private int DfsNode(HashSet<Node> visited, Node currentNode, int pathLength)
        {
            if (visited.Contains(currentNode)) return -1;

            if (currentNode.Label == 'z')
            {
                return pathLength;
            }

            var max = -1;
            foreach (var edge in currentNode.Edges)
            {
                visited.Add(currentNode);
                var newMax = DfsNode(visited, edge.Node, pathLength + edge.Weight);
                visited.Remove(currentNode);
                if (newMax > max)
                {
                    max = newMax;
                }
            }

            return max;
        }

        private Node BuildGraph(char[][] map, out Dictionary<(int Row, int Column), Node> totalNodes)
        {
            var startColumn = map[0].Select((c, i) => (c, i)).Single(x => x.c == 'a' || x.c == '.').i;
            var endColumn = map.Last().Select((c, i) => (c, i)).Single(x => x.c == '.' || x.c == 'z').i;
            var root = new Node
            {
                Index = (Row: 0, Column: startColumn),
                Label = 'a',
                Number = 0
            };

            var first = root;

            var nodes = new Dictionary<(int Row, int Column), Node>();
            totalNodes = nodes;

            nodes[root.Index] = root;

            var end = new Node
            {
                Index = (Row: map.Length - 1, Column: endColumn),
                Label = 'z',
                Number = -1
            };
            nodes[end.Index] = end;

            for (var i = 0; i < map.Length; i++)
            for (var j = 0; j < map[0].Length; j++)
            {
                if (map[i][j] == '#') continue;
                if (nodes.ContainsKey((i, j))) continue;

                var neighbours = Extensions.GetVerticalHorizontalNeighbours(map, (i, j))
                    .Where(x => x.Item != '#')
                    .ToArray();

                if (neighbours.Length <= 2) continue;

                nodes[(i, j)] = new Node
                {
                    Index = (i, j),
                    Label = map[i][j],
                    Number = -1
                };
            }

            end.Number = nodes.Count - 1;

            var last = end;

            var nodeNumber = 2;
            foreach (var node in nodes)
            {
                AddEdges(nodes, node.Value, map, first, last);
                if (node.Value.Number == -1)
                {
                    node.Value.Number = nodeNumber++;
                }
            }

            return root;
        }

        private void AddEdges(Dictionary<(int Row, int Column), Node> nodes, Node node, char[][] map, Node first,
            Node last)
        {
            var neighbours = Extensions.GetVerticalHorizontalNeighbours(map, node.Index)
                .Where(x => x.Item != '#')
                .Select(x => x.Index).ToArray();
            foreach (var neighbour in neighbours)
            {
                var prevIndex = node.Index;
                var currentIndex = neighbour;
                var length = 0;

                while (true)
                {
                    ++length;
                    var nextIndex = Extensions
                        .GetVerticalHorizontalNeighbours(map, currentIndex)
                        .Where(x => x.Item != '#')
                        .Select(x => x.Index)
                        .Single(x => x != prevIndex);

                    if (nodes.TryGetValue(nextIndex, out var endNode))
                    {
                        if (node == first)
                        {
                            endNode.Number = 1;
                        }

                        if (node == last)
                        {
                            endNode.Number = last.Number - 1;
                        }

                        node.Edges.Add(new Edge
                        {
                            Node = endNode,
                            Weight = length + 1
                        });
                        break;
                    }

                    prevIndex = currentIndex;
                    currentIndex = nextIndex;
                }
            }
        }

        [DebuggerDisplay("({Number}{Label.ToString()})")]
        class Node
        {
            public Node()
            {
                Edges = new HashSet<Edge>();
            }

            public (int Row, int Column) Index { get; set; }
            public char Label { get; set; }
            public int Number { get; set; }
            public HashSet<Edge> Edges { get; set; }
        }

        [DebuggerDisplay("({Weight} -> {Node})")]
        struct Edge
        {
            public int Weight { get; set; }
            public Node Node { get; set; }
        }
    }
}