using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task06_2
    {
        [Test]
        [TestCase(
            @"Time:      7  15   30
Distance:  9  40  200",
            71503)]
        [TestCase(@"Task06.txt", 34454850)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var lines = input.SplitLines();
            var ts = long.Parse(lines[0].SplitEmpty(":")[1].SplitEmpty(" ").JoinToString());
            var ls = long.Parse(lines[1].SplitEmpty(":")[1].SplitEmpty(" ").JoinToString());

            var result = 1L;

            var T = ts;
            var L = ls;

            var D = T * T - 4 * L;
            var x1 = (T + Math.Sqrt(D)) / 2;
            var x2 = (T - Math.Sqrt(D)) / 2;

            var xs = new[] { x1, x2, }
                .Where(x => x > 0 && x < T)
                .Concat(new[] { 0.1, T - 0.1 }) //oh shit!
                .OrderBy(x => x)
                .ToArray();

            var sign = 1;
            for (var j = 1; j < xs.Length; ++j)
            {
                if (sign < 0)
                {
                    var prev = xs[j - 1];
                    var current = xs[j];

                    var currentInt = (long)Math.Floor(current);
                    var prevInt = (long)Math.Ceiling(prev);

                    if (prevInt == prev) prevInt++;
                    if (currentInt == current) currentInt--;

                    result *= currentInt - prevInt + 1;
                }

                sign = -sign;
            }

            result.Should().Be(expected);
        }
    }
}