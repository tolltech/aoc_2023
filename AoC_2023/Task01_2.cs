using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task01_2
    {
        [Test]
        [TestCase(
            @"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000",
            45000)]
        [TestCase(
            @"Task01.txt"
            ,
            200116)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var max = input.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
                .Select(x => x.Sum())
                .OrderByDescending(x => x)
                .Take(3)
                .Sum();

            max.Should().Be(expected);
        }
    }
}