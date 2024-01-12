﻿using System.Collections;
using System.Diagnostics;
using System.Text;
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
        [TestCase(@"Task23.txt", 0)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;
            input = input.Replace(">", ".").Replace("v", ".");
            var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

            var root = BuildGraph(map);
            
            max.Should().Be(expected);
        }

        private static int max = 0;
        private static int NodeNumber = 0;

        private Node BuildGraph(char[][] map)
        {
            var startColumn = map[0].Select((c, i) => (c, i)).Single(x => x.c == 'a' || x.c == '.').i;
            var endColumn = map.Last().Select((c, i) => (c, i)).Single(x => x.c == '.' || x.c == 'z').i;
            var root = new Node
            {
                Index = (Row: 0, Column: startColumn),
                Label = 'a',
                Number = NodeNumber++
            };

            var nodes = new Dictionary<(int Row, int Column), Node>();

            nodes[root.Index] = root;

            for (var i = 0; i < map.Length; i++)
            for (var j = 0; j < map[0].Length; j++)
            {
                if (map[i][j] == '#') continue;
                
                var neighbours = Extensions.GetVerticalHorizontalNeighbours(map, (i, j))
                    .Where(x => x.Item != '#')
                    .ToArray();
                
                if (neighbours.Length <= 2) continue;

                nodes[(i, j)] = new Node
                {
                    Index = (i, j),
                    Label = map[i][j],
                    Number = NodeNumber++
                };
            }
            
            var end = new Node
            {
                Index = (Row: map.Length - 1, Column: endColumn),
                Label = 'z',
                Number = NodeNumber++
            };
            nodes[end.Index] = end;

            foreach (var node in nodes)
            {
                AddEdges(nodes, node.Value, map);
            }

            return root;
        }

        private void AddEdges(Dictionary<(int Row, int Column),Node> nodes, Node node, char[][] map)
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