using System.Diagnostics;
using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    [NonParallelizable]
    public class Task25
    {
        [Test]
        [TestCase(
            @"jqt: rhn xhk nvd
rsh: frs pzl lsr
xhk: hfx
cmg: qnr nvd lhk bvb
rhn: xhk bvb hfx
bvb: xhk hfx
pzl: lsr hfx nvd
qnr: nvd
ntq: jqt hfx bvb xhk
nvd: lhk
lsr: lhk
rzs: qnr cmg lsr rsh
frs: qnr lhk lsr", 54)]
        [TestCase(@"Task25.txt", 0)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var (nodes, edges) = ParseGraph(input);

            var bridges = new HashSet<Edge>();

            for (var i = 0; i < edges.Length; i++)
            {
                for (var j = i + 1; j < edges.Length; j++)
                {
                    var except = new HashSet<Edge>(new[]
                    {
                        edges[i], edges[j],
                        Revert(edges[i]), Revert(edges[j])
                    });

                    // if (i != 4 || j != 19)
                    // {
                    //     continue;
                    // }
                    
                    var bridge = FindBridge(nodes, except);

                    if (bridge == null) continue;

                    bridges = except;
                    bridges.Add(bridge.Value);
                    bridges.Add(Revert(bridge.Value));
                    break;
                }

                if (bridges.Count == 6) break;
            }

            var result = GetComponentsCount(nodes, bridges);

            result.Should().Be(expected);
        }

        private static (Dictionary<string, Node> nodes, Edge[] edges) ParseGraph(string input)
        {
            var totalEdges = new HashSet<Edge>();
            var nodes = new Dictionary<string, Node>();
            var number = 0;
            foreach (var line in input.SplitLines())
            {
                var split = line.SplitEmpty(": ");
                var nodeLabel = split[0];
                var children = split[1].SplitEmpty(" ");

                if (!nodes.TryGetValue(nodeLabel, out var node))
                {
                    node = new Node(nodeLabel, number++);
                    nodes[nodeLabel] = node;
                }

                foreach (var child in children)
                {
                    if (!nodes.TryGetValue(child, out var childNode))
                    {
                        childNode = new Node(child, number++);
                        nodes[child] = childNode;
                    }

                    totalEdges.Add(new Edge(node, childNode));
                    totalEdges.Add(new Edge(childNode, node));

                    node.Children.Add(childNode);
                    childNode.Children.Add(node);
                }
            }

            var edgesDistinct = new HashSet<Edge>();
            foreach (var edge in totalEdges)
            {
                if (edgesDistinct.Contains(edge) || edgesDistinct.Contains(Revert(edge)))
                    continue;

                edgesDistinct.Add(edge);
            }

            var edges = edgesDistinct.ToArray();
            return (nodes, edges);
        }

        private Edge? FindBridge(Dictionary<string, Node> nodes, HashSet<Edge> except)
        {
            deepLevels = Enumerable.Repeat(-1, nodes.Count).ToArray();
            minDeepLevels = deepLevels.ToArray();
            
            DfsNode(null, nodes.Values.First(), 0, except);

            return bridgeGlobal;
        }

        private static int[] deepLevels = Array.Empty<int>();
        private static int[] minDeepLevels = Array.Empty<int>();
        private static Edge? bridgeGlobal;

        private int DfsNode(Node? prevNode, Node nextNode, int level, HashSet<Edge> except)
        {
            if (deepLevels[nextNode.Number] >= 0)
            {
                return Math.Min(deepLevels[nextNode.Number], minDeepLevels[prevNode!.Number]);
            }

            deepLevels[nextNode.Number] = level;
            minDeepLevels[nextNode.Number] = level;

            foreach (var child in nextNode.Children)
            {
                var s = 1;
                
                if (child == prevNode) continue;
                if (except.Contains(new Edge(nextNode, child))) continue;

                var newMin = DfsNode(nextNode, child, level + 1, except);
                if (newMin < minDeepLevels[nextNode.Number])
                {
                    minDeepLevels[nextNode.Number] = newMin;
                }
            }

            if (prevNode != null && !except.Contains(new Edge(prevNode, nextNode)) && deepLevels[prevNode.Number] < minDeepLevels[nextNode.Number])
            {
                bridgeGlobal = new Edge(prevNode, nextNode);
            }

            return minDeepLevels[nextNode.Number];
        }

        private long GetComponentsCount(Dictionary<string, Node> nodes, HashSet<Edge> bridges)
        {
            var visited = new HashSet<Node>();
            var queue = new Queue<Node>();
            queue.Enqueue(nodes.First().Value);

            while (queue.Count > 0)
            {
                var element = queue.Dequeue();
                if(visited.Contains(element)) continue;

                visited.Add(element);
                foreach (var child in element.Children)
                {
                    if (visited.Contains(child)) continue;
                    if (bridges.Contains(new Edge(element, child))) continue;

                    queue.Enqueue(child);
                }
            }

            return 1L * visited.Count * (nodes.Count - visited.Count);
        }

        private static Edge Revert(Edge edge) => new Edge(edge.To, edge.From);

        [DebuggerDisplay("{From} {To}")]
        struct Edge
        {
            public Edge(Node from, Node to)
            {
                From = from;
                To = to;
            }
            
            public Node From { get; set; }
            public Node To { get; set; }
        }

        [DebuggerDisplay("{Number} '{Label}'")]
        private class Node
        {
            public Node(string label, int number)
            {
                Number = number;
                Label = label;
                Children = new HashSet<Node>();
            }

            public string Label { get; }
            public int Number { get; set; }
            public HashSet<Node> Children { get; }
        }

        [Test]
        [TestCase(@"1: 2 3 9 10
2: 3
3: 4
4: 5 8
5: 6 7
6: 7
7: 8
8: 4
9: 10","3 4")]
        [TestCase(@"1: 2 3
2: 3 5
3: 4
4: 5
5: 6 7
6: 7
7: 8","7 8")]
        [TestCase(@"1: 2 3
2: 3 4
4: 5 6
5: 6 3","", true)]
        public void TestTest(string input, string expected, bool notFoundExpected = false)
        {
            bridgeGlobal = null;
            var (nodes, edges) = ParseGraph(input);

            var bridge = FindBridge(nodes, new HashSet<Edge>());

            if (notFoundExpected)
            {
                bridge.Should().BeNull();
                return;
            }
            
            bridge.Should().NotBeNull();
            string.Join(" ", new[] { bridge!.Value.From.Label, bridge.Value.To.Label }.OrderBy(x => x)).Should().Be(expected);
        }
    }
}