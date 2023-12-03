using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task03
    {
        [Test]
        [TestCase(
            @"467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598..",
            4361)]
        [TestCase(@"Task03.txt", 540025)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var map = input.SplitEmpty("\r", "\n").Select(x => x.ToArray()).ToArray();
            var result = 0;

            var numbers = new Dictionary<int, HashSet<(int Item, (int Row, int Col) Start)>>();
            for (var row = 0; row < map.Length; ++row)
            for (var col = 0; col < map[0].Length; col++)
            {
                var c = map[row][col];
                if (!char.IsDigit(c)) continue;

                var number = map[row].Skip(col).TakeWhile(char.IsDigit).ToArray();
                numbers[row] = numbers!.SafeGet(row) ?? new HashSet<(int Item, (int Row, int Col) Start)>();
                numbers[row].Add((int.Parse(number), (row, col)));
                col += number.Length - 1;
            }

            var connectedNumberIndices = new HashSet<(int Row, int Col)>();

            for (var row = 0; row < map.Length; ++row)
            for (var col = 0; col < map[0].Length; col++)
            {
                var c = map[row][col];
                if (char.IsDigit(c) || c == '.') continue;

                foreach (var number in Extensions.GetAllNeighbours(map, (row, col))
                             .Where(x => char.IsDigit(x.Item)))
                {
                    connectedNumberIndices.Add(number.Index);
                }
            }

            var connectedNumbers = new HashSet<(int Number, (int Row, int Col) Index)>();
            foreach (var connectedNumberIndex in connectedNumberIndices)
            {
                var row = connectedNumberIndex.Row;
                var col = connectedNumberIndex.Col;
                var number = numbers[row].First(x => x.Start.Col <= col && (x.Start.Col + x.Item.ToString().Length) > col);
                connectedNumbers.Add(number);
            }

            result = connectedNumbers.Select(x => x.Number).Sum();

            result.Should().Be(expected);

        }
    }
}