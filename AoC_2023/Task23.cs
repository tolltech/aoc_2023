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
#####################.#",
            94)]
        [TestCase(@"Task23.txt", 0)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

            //var visited = new HashSet<(int Row, int Column)>();
            //var paths = new Dictionary<(int Row, int Column), int>();
            // var startColumn = map[0].Select((c, i) => (c, i)).Single(x => x.c == '.').i;
            var endColumn = map.Last().Select((c, i) => (c, i)).Single(x => x.c == '.').i;
            // Dfs(visited, paths, map, (Row: 0, Column: startColumn), (Row: map.Length - 1, Column: endColumn), 0);

            var root = BuildGraph(map, out var nodeMap);

            var pathLengths = new Dictionary<Node, int>();
            DfsNode(new HashSet<Node>(), pathLengths, root, 0);
            var result = pathLengths[nodeMap[(map.Length - 1, endColumn)]];
            result.Should().Be(expected);
        }

        private static int NodeNumber;
        
        private Node BuildGraph(char[][] map, out Dictionary<(int Row, int Column), Node> nodeMap)
        {
            nodeMap = new Dictionary<(int Row, int Column), Node>();
            var startColumn = map[0].Select((c, i) => (c, i)).Single(x => x.c == '.').i;
            var endColumn = map.Last().Select((c, i) => (c, i)).Single(x => x.c == '.').i;
            var root = new Node
            {
                Index = (Row: 0, Column: startColumn),
                Label = '.',
                Number = NodeNumber++
            };

            BuildGraph(nodeMap, map, root, (1, startColumn), (Row: map.Length - 1, Column: endColumn));

            return root;
        }

        private void DfsNode(HashSet<Node> visited, Dictionary<Node, int> pathLengths, Node currentNode, int pathLength)
        {
            if (visited.Contains(currentNode)) return;

            if (!pathLengths.ContainsKey(currentNode) || pathLengths[currentNode] < pathLength)
            {
                pathLengths[currentNode] = pathLength;
            }
            
            foreach (var edge in currentNode.Edges)
            {
                visited.Add(currentNode);
                DfsNode(visited, pathLengths, edge.Node, pathLength + edge.Weight);
                visited.Remove(currentNode);
            }
        }
        
        private void BuildGraph(Dictionary<(int Row, int Column), Node> nodes, char[][] map, Node currentNode, (int Row, int Column) nextStep, (int Row, int Column) endIndex)
        {
            var currentIndex = currentNode.Index;
            nodes[currentIndex] = currentNode;

            var weight = 1;
            var prevIndex = currentIndex;
            currentIndex = nextStep;
            
            while (true)
            {
                var currentItem = map[currentIndex.Row][currentIndex.Column];
                var neighbours = !directions.TryGetValue(currentItem, out var dir)
                    ? Extensions.GetVerticalHorizontalNeighbours(map, currentIndex)
                        .Where(x => x.Item == '.' || char.IsLetterOrDigit(x.Item) || directions.ContainsKey(x.Item))
                        .Select(x => x.Index).ToArray()
                    : new[] { (currentIndex.Row + dir.Item1, currentIndex.Column + dir.Item2) };

                neighbours = neighbours.Where(x => x != prevIndex).ToArray();

                if (neighbours.Length == 0)
                {
                    if (currentIndex == endIndex)
                    {
                        var endNode = nodes!.SafeGet(currentIndex) ?? new Node
                        {
                            Label = map[currentIndex.Row][currentIndex.Column],
                            Index = currentIndex,
                            Number = NodeNumber++
                        };

                        nodes[endNode.Index] = endNode;
                        
                        currentNode.Edges.Add(new Edge
                        {
                            Weight = weight,
                            Node = endNode
                        });
                    }
                    return;
                }

                if (neighbours.Length == 1)
                {
                    weight++;
                    prevIndex = currentIndex;
                    currentIndex = neighbours.Single();
                    continue;
                }

                var newNode = nodes!.SafeGet(currentIndex) ?? new Node
                {
                    Index = currentIndex,
                    Label = map[currentIndex.Row][currentIndex.Column],
                    Number = NodeNumber++
                };

                currentNode.Edges.Add(new Edge
                {
                    Weight = weight,
                    Node = newNode
                });

                foreach (var neighbour in neighbours)
                {
                    BuildGraph(nodes, map, newNode, neighbour, endIndex);
                }

                return;
            }
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
                if (neighbour.Item != '.' && !char.IsLetterOrDigit(neighbour.Item) && !directions.ContainsKey(neighbour.Item)) continue;

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