using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task10_2
    {
        [Test]
        [TestCase(
            @"...........
.S-------7.
.|F-----7|.
.||.....||.
.||.....||.
.|L-7.F-J|.
.|..|.|..|.
.L--J.L--J.
...........",
            4)]
        [TestCase(
            @"..........
.S------7.
.|F----7|.
.||OOOO||.
.||OOOO||.
.|L-7F-J|.
.|II||II|.
.L--JL--J.
..........",
            4)]
        [TestCase(
            @".F----7F7F7F7F-7....
.|F--7||||||||FJ....
.||.FJ||||||||L7....
FJL7L7LJLJ||LJ.L-7..
L--J.L7...LJS7F-7L7.
....F-J..F7FJ|L7L7L7
....L7.F7||L7|.L7L7|
.....|FJLJ|FJ|F7|.LJ
....FJL-7.||.||||...
....L---J.LJ.LJLJ...",
            8)]
        [TestCase(
            @"....................
FF7FSF7F7F7F7F7F---7
L|LJ||||||||||||F--J
FL-7LJLJ||||||LJL-77
F--JF--7||LJLJ7F7FJ-
L---JF-JLJ.||-FJLJJ7
|F|F-JF---7F7-L7L|7|
|FFJF7L7F-JF7|JL---7
7-L-JL7||F7|L7F-7F7|
L.L7LFJ|||||FJL7||LJ
L7JLJL-JLJLJL--JLJ.L",
            10)]
        [TestCase(@"Task10.txt", 0)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.Replace("O", ".").Replace("I", ".").SplitLines().Select(x => x.ToArray()).ToArray();
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

            var oldLoop = GetLoop(map, start);

            var startItem = ('S', start);

            var downStep = MakeStep(start, DownStep);
            var upStep = MakeStep(start, TopStep);
            var leftStep = MakeStep(start, LeftStep);
            var rightStep = MakeStep(start, RightStep);

            var stepsS = new List<(int, int)>();
            if (IsConnected(startItem, (map[downStep.Item1][downStep.Item2], downStep))) stepsS.Add(DownStep);
            if (IsConnected(startItem, (map[upStep.Item1][upStep.Item2], upStep))) stepsS.Add(TopStep);
            if (IsConnected(startItem, (map[leftStep.Item1][leftStep.Item2], leftStep))) stepsS.Add(LeftStep);
            if (IsConnected(startItem, (map[rightStep.Item1][rightStep.Item2], rightStep))) stepsS.Add(RightStep);

            if (stepsS.Count != 2) throw new NotImplementedException();

            var sReplace = AvailableSteps.Where(x => x.Value.Length == 2)
                .Single(x => x.Value.Intersect(stepsS).Count() == 2)
                .Key;
            map[start.Row][start.Column] = sReplace;

            var scaledMap = new (char Item, bool Real)[map.Length * 3][];
            for (var i = 0; i < scaledMap.Length; ++i)
            {
                scaledMap[i] = new (char Item, bool Real)[map[0].Length * 3];
            }

            for (var i = 0; i < map.Length; ++i)
            for (var j = 0; j < map[i].Length; j++)
            {
                var newI = 1 + 3 * i;
                var newJ = 1 + 3 * j;
                var c = map[i][j];
                scaledMap[newI][newJ] = (c, true);

                for (var ii = -1; ii <= 1; ++ii)
                for (var jj = -1; jj <= 1; ++jj)
                {
                    if (ii == 0 && jj == 0) continue;
                    scaledMap[newI + ii][newJ + jj] = ('.', false);
                }

                if (!oldLoop.Contains((i, j))) continue;

                var availableSteps = AvailableSteps[c];
                foreach (var step in availableSteps)
                {
                    var newStep = MakeStep((newI, newJ), step);
                    var newC = step == LeftStep || step == RightStep
                        ? '-'
                        : '|';
                    scaledMap[newStep.Item1][newStep.Item2] = (newC, false);
                }
            }

            var s = Print(scaledMap);
            //--------------------------------------------------------------

            start = (1 + 3 * start.Row, 1 + 3 * start.Column);

            var loop = new HashSet<(int Row, int Column)> { start };
            var queue = new Queue<(int Row, int Column)>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var connectedNeighbours = Extensions.GetVerticalHorizontalNeighbours(scaledMap, current)
                    .Where(x => !loop.Contains(x.Index))
                    .Where(x => IsConnected((x.Item.Item, x.Index),
                        ((scaledMap[current.Row][current.Column].Item), current)))
                    .ToArray();

                foreach (var neighbour in connectedNeighbours)
                {
                    loop.Add(neighbour.Index);
                    queue.Enqueue(neighbour.Index);
                }
            }

            var startNeighbours = Extensions.GetAllNeighbours(scaledMap, start);

            var result = 0;
            foreach (var startNeighbour in startNeighbours)
            {
                if (startNeighbour.Item.Item != '.') continue;

                var area = GetArea(scaledMap, startNeighbour.Index, loop);
                if (!area.Edge)
                {
                    result = area.Area.Count(x => scaledMap[x.Row][x.Column].Real);
                    break;
                }
            }

            result.Should().Be(expected);
        }

        private HashSet<(int, int)> GetLoop(char[][] map, (int Row, int Column) start)
        {
            var loop = new HashSet<(int Row, int Column)> { start };
            var queue = new Queue<(int Row, int Column)>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var connectedNeighbours = Extensions.GetVerticalHorizontalNeighbours(map, current)
                    .Where(x => !loop.Contains(x.Index))
                    .Where(x => IsConnected((x.Item, x.Index),
                        ((map[current.Row][current.Column]), current)))
                    .ToArray();

                foreach (var neighbour in connectedNeighbours)
                {
                    loop.Add(neighbour.Index);
                    queue.Enqueue(neighbour.Index);
                }
            }

            return loop;
        }

        private static string Print((char Item, bool Real)[][] map, HashSet<(int, int)>? xs = null)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < map.Length; i++)
            {
                var row = map[i];
                for (var index = 0; index < row.Length; index++)
                {
                    var tuple = row[index];
                    sb.Append(xs?.Contains((i, index)) ?? false ? tuple.Real ? 'R' : 'X' :
                        tuple.Real && tuple.Item == '.' ? 'r' : tuple.Item);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private (HashSet<(int Row, int Column, bool Real, char Item)> Area, bool Edge) GetArea(
            (char Item, bool Real)[][] map, (int i, int j) point, HashSet<(int Row, int Column)> loop)
        {
            var queue = new Queue<(int i, int j)>();
            queue.Enqueue(point);
            var visited = new HashSet<(int, int)>();
            var edge = false;
            var other = false;
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (visited.Contains(current)) continue;

                visited.Add(current);
                var s = Print(map, visited);
                var neighbours = Extensions.GetVerticalHorizontalNeighbours(map, current).ToArray();
                if (neighbours.Length != 4)
                {
                    edge = true;
                }

                foreach (var next in neighbours.Where(x => !loop.Contains(x.Index)))
                {
                    queue.Enqueue(next.Index);
                }
            }

            var s2 = Print(map, visited);

            return (
                new HashSet<(int Row, int Column, bool Real, char Item)>(visited.Select(x =>
                    (x.Item1, x.Item2, map[x.Item1][x.Item2].Real, map[x.Item1][x.Item2].Item))), edge);
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