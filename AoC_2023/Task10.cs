using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task10
    {
        [Test]
        [TestCase(
            @".....
.S-7.
.|.|.
.L-J.
.....",
            4)]
        [TestCase(
            @"..F7.
.FJ|.
SJ.L7
|F--J
LJ...",
            8)]
        [TestCase(@"Task10.txt", 6806)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines().Select(x => x.ToArray()).ToArray();
            var start = (Row: 0, Column: 0);

            for (var i = 0; i < map.Length; ++i)
            for (var j = 0; j < map[i].Length; j++)
            {
                if (map[i][j] == 'S')
                {
                    start = (i, j);
                    break;
                }
            }

            var visited = new Dictionary<(int Row, int Column), int>();
            visited[start] = 0;
            var queue = new Queue<(int Row, int Column)>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var l = visited[current];
                var connectedNeighbours = Extensions.GetVerticalHorizontalNeighbours<char>(map, current)
                    .Where(x => !visited.ContainsKey(x.Index))
                    .Where(x => IsConnected(x, (map[current.Row][current.Column], current)))
                    .ToArray();

                foreach (var neighbour in connectedNeighbours)
                {
                    visited[neighbour.Index] = l + 1;
                    queue.Enqueue(neighbour.Index);
                }
            }

            var result = visited.Values.Max();
            result.Should().Be(expected);
        }

        private bool IsConnected((char Item, (int Row, int Col) Index) one, (char Item, (int Row, int Col) Index) other)
        {
            var oneAvailable = AvailableSteps[one.Item].Any(step => MakeStep(one.Index, step) == other.Index);
            var otherAvailable = AvailableSteps[other.Item].Any(step => MakeStep(other.Index, step) == one.Index);
            return oneAvailable && otherAvailable;
        }

        private static readonly (int Row, int Column) DownStep = (1, 0);
        private static readonly (int Row, int Column) LeftStep = (0, -1);
        private static readonly (int Row, int Column) TopStep = (-1, 0); 
        private static readonly (int Row, int Column) RightStep = (0, 1);

        private static (int, int) MakeStep((int, int) from, (int, int) step)
        {
            return (from.Item1 + step.Item1, from.Item2 + step.Item2);
        }

        private static readonly Dictionary<char, (int Row, int Column)[]> AvailableSteps = new()
        {
            { '|', new[] { DownStep, TopStep } },
            { '-', new[] { LeftStep, RightStep } },
            { 'L', new[] { TopStep, RightStep } },
            { 'J', new[] { LeftStep, TopStep } },
            { '7', new[] { LeftStep, DownStep } },
            { 'F', new[] { DownStep, RightStep } },
            { '.', Array.Empty<(int Row, int Column)>() },
            { 'S', new[] { DownStep, RightStep, LeftStep, TopStep } },
        };
    }
}