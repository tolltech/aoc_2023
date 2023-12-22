using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task16
    {
        [Test]
        [TestCase(
            @".|...\....
|.-.\.....
.....|-...
........|.
..........
.........\
..../.\\..
.-.-/..|..
.|....-|.\
..//.|....",
            46)]
        [TestCase(@"Task16.txt", 6740)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitLines();
            var visited = new HashSet<((int, int) InputDirection, (int, int) Index)>();

            MakeStep(map, visited, (0, 0), RightStep);

            var result = visited.Select(x => x.Index).Distinct().Count();

            result.Should().Be(expected);
        }

        private void MakeStep(string[] map, HashSet<((int, int) InputDirection, (int, int) Index)> visited,
            (int Row, int Column) position, (int Row, int Column) direction)
        {
            if (position.Row < 0 || position.Row >= map.Length || position.Column < 0 ||
                position.Column >= map[0].Length) return;

            var state = (direction, position);
            if (visited.Contains(state)) return;

            visited.Add(state);

            var directions = GetNewDirections(map[position.Row][position.Column], direction);

            foreach (var newDirection in directions)
            {
                MakeStep(map, visited, (position.Row + newDirection.Row, position.Column + newDirection.Column),
                    newDirection);
            }
        }

        private IEnumerable<(int Row, int Column)> GetNewDirections(char c, (int Row, int Column) direction)
        {
            switch (c)
            {
                case '.': yield return direction;
                    break;
                case '/':
                    if (direction == RightStep) yield return UpStep;
                    if (direction == LeftStep) yield return DownStep;
                    if (direction == UpStep) yield return RightStep;
                    if (direction == DownStep) yield return LeftStep;
                    break;
                case '\\':
                    if (direction == RightStep) yield return DownStep;
                    if (direction == LeftStep) yield return UpStep;
                    if (direction == UpStep) yield return LeftStep;
                    if (direction == DownStep) yield return RightStep;
                    break;
                case '-':
                    if (direction == RightStep) yield return RightStep;
                    if (direction == LeftStep) yield return LeftStep;
                    if (direction == UpStep || direction == DownStep)
                    {
                        yield return LeftStep;
                        yield return RightStep;
                    }
                    break;
                case '|':
                    if (direction == UpStep) yield return UpStep;
                    if (direction == DownStep) yield return DownStep;
                    if (direction == LeftStep || direction == RightStep)
                    {
                        yield return UpStep;
                        yield return DownStep;
                    }
                    break;
                default: throw new NotImplementedException();
            }
        }

        public static readonly (int Row, int Column) DownStep = (1, 0);
        public static readonly (int Row, int Column) LeftStep = (0, -1);
        public static readonly (int Row, int Column) UpStep = (-1, 0);
        public static readonly (int Row, int Column) RightStep = (0, 1);
    }
}