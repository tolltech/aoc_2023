using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task03_2
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
            467835)]
        [TestCase(@"Task03.txt", 84584891)]
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

            for (var row = 0; row < map.Length; ++row)
            for (var col = 0; col < map[0].Length; col++)
            {
                var c = map[row][col];
                if (c != '*') continue;

                var set = new HashSet<(int, (int, int))>();
                foreach (var number in Extensions.GetAllNeighbours(map, (row, col))
                             .Where(x => char.IsDigit(x.Item)))
                {
                    var nCol = number.Index.Col;
                    var nRow = number.Index.Row;
                    var connectedNumber = numbers[nRow].First(x => x.Start.Col <= nCol && (x.Start.Col + x.Item.ToString().Length) > nCol);
                    set.Add(connectedNumber);
                }

                if (set.Count == 2)
                {
                    result += set.First().Item1 * set.Last().Item1;
                }
            }

            result.Should().Be(expected);

        }
    }
}